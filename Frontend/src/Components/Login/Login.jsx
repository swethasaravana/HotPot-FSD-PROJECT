import { useState, useRef, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { LoginModel } from "../../Models/Login";
import { userLoginAPICall } from "../../Services/AuthenticationService";
import logo from '../../assets/logo.png';
import './Login.css';
import { FaEnvelope, FaLock, FaEye, FaEyeSlash, FaTimes } from 'react-icons/fa';

const Login = () => {
  const [user, setUser] = useState(new LoginModel());
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState({ message: '', show: false });
  const [isLoading, setIsLoading] = useState(false);
  const [buttonPosition, setButtonPosition] = useState({ x: 0, y: 0 });
  const [isButtonRunning, setIsButtonRunning] = useState(false);
  const buttonRef = useRef(null);
  const navigate = useNavigate();

  const areFieldsFilled = user.username.trim() !== '' && user.password.trim() !== '';

  const changeUser = (event) => {
    const { name, value } = event.target;
    setUser(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const login = async (event) => {
    event.preventDefault();
    if (!areFieldsFilled) return;

    setIsLoading(true);
    setError({ message: '', show: false });

    try {
      const response = await userLoginAPICall(user);

      if (response.status === 200) {
        const loggedInUser = response.data.data;
        sessionStorage.setItem("user", JSON.stringify(loggedInUser));
        const role = loggedInUser.role?.toLowerCase();

        switch (role) {
          case 'admin':
            navigate("/admindashboard");
            break;
          case 'restaurantmanager':
            navigate("/restaurantmanager-dashboard");
            break;
          case 'customer':
            navigate("/customerdashboard");
            break;
          case 'deliverypartner':
            navigate("/deliverydashboard");
            break;
          default:
            setError({
              message: 'Unauthorized access. Please contact support.',
              show: true
            });
            sessionStorage.removeItem("user");
        }
      } else {
        setError({ message: 'Incorrect email or password', show: true });
      }
    } catch (error) {
      console.error("Login Error:", error);
      setError({
        message: error.response?.data?.message || 'Login failed. Please check your credentials.',
        show: true
      });
    } finally {
      setIsLoading(false);
    }
  };

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  const closeError = () => {
    setError(prev => ({ ...prev, show: false }));
  };

  const moveButton = () => {
    if (areFieldsFilled) return;

    setIsButtonRunning(true);

    const container = buttonRef.current?.parentElement?.parentElement;
    if (!container) return;

    const containerRect = container.getBoundingClientRect();
    const buttonRect = buttonRef.current.getBoundingClientRect();

    const maxX = containerRect.width - buttonRect.width;
    const maxY = containerRect.height - buttonRect.height;

    const offsetX = Math.floor(Math.random() * maxX);
    const offsetY = Math.floor(Math.random() * maxY);

    const directionX = Math.random() < 0.5 ? -1 : 1;

    setButtonPosition({
      x: directionX * offsetX,
      y: -offsetY
    });

    setTimeout(() => setIsButtonRunning(false), 500);
  };

  const handleButtonMouseEnter = () => {
    if (!areFieldsFilled && !isButtonRunning) {
      moveButton();
    }
  };

  const handleButtonClick = (e) => {
    if (!areFieldsFilled) {
      e.preventDefault();
      moveButton();
    }
  };

  useEffect(() => {
    if (areFieldsFilled) {
      setButtonPosition({ x: 0, y: 0 });
    }
  }, [user.username, user.password]);

  return (
    <div className="login-container">
      <div className="bg-animation">
        {[...Array(3)].map((_, i) => (
          <div key={`bubble-${i + 1}`} className={`bubble bubble-${i + 1}`} />
        ))}
      </div>

      <div className="login-card">
        <div className="logo">
          <img src={logo} alt="HotByte Logo"
            onError={(e) => { e.target.onerror = null; e.target.src = '/default-logo.png'; }} />
        </div>

        <h2>Login to HotByte</h2>
        <p>Get access to your orders, offers, and more!</p>

        <form onSubmit={login} noValidate>
          <div className="input-group">
            <input
              type="email"
              name="username"
              placeholder="Enter Your Email"
              required
              value={user.username}
              onChange={changeUser}
              autoComplete="username"
            />
            <FaEnvelope className="icon" />
          </div>

          <div className="input-group">
            <input
              type={showPassword ? "text" : "password"}
              name="password"
              placeholder="Enter Password"
              required
              value={user.password}
              onChange={changeUser}
              autoComplete="current-password"
            />
            <FaLock className="icon" />
            <button
              type="button"
              className="toggle-password"
              onClick={togglePasswordVisibility}
              aria-label={showPassword ? "Hide password" : "Show password"}
            >
              {showPassword ? <FaEyeSlash /> : <FaEye />}
            </button>
          </div>

          <button
            ref={buttonRef}
            type="submit"
            className="login-btn"
            disabled={isLoading}
            style={{
              transform: `translate(${buttonPosition.x}px, ${buttonPosition.y}px)`,
              transition: isButtonRunning ? 'transform 0.5s ease-out' : 'none',
              cursor: areFieldsFilled ? 'pointer' : 'default'
            }}
            onMouseEnter={handleButtonMouseEnter}
            onClick={handleButtonClick}
          >
            {isLoading ? 'Logging in...' : 'Login'}
          </button>
        </form>

        <div className="footer-links">
          {/* <a href="/forgot-password">Forgot Password?</a> */}
          <span>•</span>
          <a href="/signup">New User? Sign Up</a>
        </div>
      </div>

      {error.show && (
        <div className="popup">
          <div className="popup-content">
            <button
              className="close-btn"
              onClick={closeError}
              aria-label="Close error message"
            >
              <FaTimes />
            </button>
            <p>{error.message}</p>
          </div>
        </div>
      )}
    </div>
  );
};

export default Login;
