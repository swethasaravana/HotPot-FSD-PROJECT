import React from 'react';
import { useNavigate } from 'react-router-dom';
import './HomePage.css';
import logo from '../../assets/logo.png';

const HomePage = () => {
  const navigate = useNavigate(); 

  const handleLoginClick = () => {
    navigate('/login');
  };

  const handleStart = () => {
    navigate('/login'); 
  };

  return (
    <div className="hotbyte-container">
      <header className="hotbyte-header">
      <div className="logo-container">
          <img src={logo} alt="HotByte Logo" className="header-logo" />
        </div>
        <nav className="hotbyte-nav">
          <a href="/login" onClick={handleLoginClick} className="nav-link">Login/Register</a>
        </nav>
      </header>

      <main className="hotbyte-main">
        <section className="hero-section">
          <h2 className="hero-title">Order Your Favorite Food Anytime</h2>
          <p className="hero-description">
            Delicious meals from your favorite restaurants delivered hot and fresh.
            Browse menus, track your orders, and enjoy exclusive discounts!
          </p>
          <button className="cta-button" onClick={handleStart}>Get Started</button>
        </section>

        <section className="features-section">
          <h3 className="features-title">Our Features</h3>
          <div className="features-grid">
            <div className="feature-card">
              <h4>Easy Registration</h4>
              <p>Quick sign up for customers and restaurants with secure login.</p>
            </div>
            <div className="feature-card">
              <h4>Dynamic Menus</h4>
              <p>Breakfast to dinner, filter by taste, cuisine, or price.</p>
            </div>
            <div className="feature-card">
              <h4>Real-Time Tracking</h4>
              <p>Track your order status and get notified instantly via email.</p>
            </div>
            <div className="feature-card">
              <h4>Restaurant Management</h4>
              <p>Manage menus, prices, and orders in real-time.</p>
            </div>
            <div className="feature-card">
              <h4>Admin Controls</h4>
              <p>Admins can manage users, restaurants, and menu listings.</p>
            </div>
            <div className="feature-card">
              <h4>Instant Notifications</h4>
              <p>Get real-time alerts about order status, deals, and promotions.</p>
            </div>
          </div>
        </section>
      </main>
      
      {[...Array(10)].map((_, i) => (
        <div 
          key={i}
          className="delivery-particle"
          style={{
            width: `${Math.random() * 40 + 20}px`,
            height: `${Math.random() * 40 + 20}px`,
            left: `${Math.random() * 100}%`,
            top: `${Math.random() * 100}%`,
            backgroundImage: `url('https://cdn-icons-png.flaticon.com/512/869/869869.png')`,
            animationDuration: `${Math.random() * 20 + 10}s`,
            animationDelay: `${Math.random() * 5}s`,
            opacity: Math.random() * 0.5 + 0.2
          }}
        />
      ))} 

      <footer className="hotbyte-footer">
        &copy; 2025 HotByte Delivery App. All Rights Reserved.
      </footer>
    </div>
  );
};

export default HomePage;