import React, { useEffect, useState } from "react";
import {
  getCartByCustomerId,
  getCustomerAddresses,
  addCustomerAddress,
  placeOrder,
} from "../../../Services/CustomerService";
import "./CheckoutPage.css";
import qrCode from "../../../assets/HotByte UPI QR.png";
import logo from '../../../assets/logo.png';
import { useNavigate } from "react-router-dom";



const CheckoutPage = () => {
  const user = JSON.parse(sessionStorage.getItem("user"));
  const customerId = user?.id;

  const [cart, setCart] = useState([]);
  const [addresses, setAddresses] = useState([]);
  const [newAddress, setNewAddress] = useState({
    label: "",
    street: "",
    city: "",
    pincode: "",
  });
  const [selectedAddress, setSelectedAddress] = useState(null);
  const [paymentMethod, setPaymentMethod] = useState("UPI");
  const [paymentStatus, setPaymentStatus] = useState("pending"); // 'pending', 'processing', 'success', 'failed'
  const [orderPlaced, setOrderPlaced] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    loadCart();
    loadAddresses();
  }, []);

  const loadCart = async () => {
    try {
      const res = await getCartByCustomerId(customerId);
      setCart(res.data?.items || []);
    } catch (err) {
      console.error("Cart fetch error", err);
      setCart([]);
    }
  };

  const loadAddresses = async () => {
    try {
      const res = await getCustomerAddresses(customerId);
      setAddresses(res.data || []);
      if (res.data?.length > 0) {
        setSelectedAddress(res.data[0].addressId);
      }
    } catch (err) {
      console.error("Address fetch error", err);
    }
  };

  const handleAddAddress = async () => {
    const { label, street, city, pincode } = newAddress;
    if (!label || !street || !city || !pincode) {
      alert("All fields are required.");
      return;
    }

    try {
      await addCustomerAddress(customerId, newAddress);
      alert("Address added successfully!");
      setNewAddress({ label: "", street: "", city: "", pincode: "" });
      loadAddresses();
    } catch (err) {
      alert("Error adding address. Please try again.");
    }
  };

  const getPaymentMethodId = (method) => {
    const methods = {
      "UPI": 1,
      "Cash on Delivery": 2,
      "Card": 3
    };
    return methods[method] || 1;
  };

  const processPayment = async () => {
    if (!selectedAddress) {
      alert("Please select a delivery address.");
      return;
    }

    setPaymentStatus("processing");

    try {
      // Simulate payment processing
      await new Promise(resolve => setTimeout(resolve, 1500));
      
      // In a real app, this would be replaced with actual payment gateway integration
      // For example: Razorpay, Stripe, etc.
      if (paymentMethod === "Cash on Delivery") {
        // No payment processing needed for COD
        await confirmOrder();
      } else {
        // For other payment methods, we'd typically redirect to payment gateway
        // This is just a simulation - in real app use payment gateway SDK
        const paymentSuccess = simulatePaymentGateway();
        
        if (paymentSuccess) {
          setPaymentStatus("success");
          await confirmOrder();
        } else {
          throw new Error("Payment failed");
        }
      }
    } catch (error) {
      console.error("Payment error:", error);
      setPaymentStatus("failed");
      alert("Payment failed. Please try again.");
    }
  };

  // Simulates payment gateway response (replace with real integration)
  const simulatePaymentGateway = () => {
    return Math.random() > 0.1; // 90% success rate for demo
  };

  const confirmOrder = async () => {
    const payload = {
      customerId,
      customerAddressId: selectedAddress,
      paymentMethodId: getPaymentMethodId(paymentMethod),
      paymentStatusId: paymentMethod === "Cash on Delivery" ? 1 : 2, // 1 = Pending, 2 = Paid
    };

    try {
      await placeOrder(payload);
      setOrderPlaced(true);
      setCart([]);
    } catch (error) {
      console.error("Order placement failed:", error);
      alert("Failed to place order. Please try again.");
      setPaymentStatus("failed");
    }
  };

  const cartTotal = cart.reduce(
    (sum, item) => sum + item.quantity * item.priceAtPurchase,
    0
  );

  if (orderPlaced) {
    return (
      <div className="order-success">
        <h2>Order Placed Successfully!</h2>
        <p>Thank you for your order. Your food will be delivered soon.</p>
        <button onClick={() => navigate('/track-orders')}>Track Orders</button>
        <button onClick={() => window.location.href = "/customerdashboard"}>Back to Dashboard</button>

      </div>
    );
  }

  return (
    <div className="checkout-container">

        <div className="logo">
          <img src={logo} alt="HotByte Logo" 
          onError={(e) => {e.target.onerror = null; e.target.src = '/default-logo.png';}}/>
        </div>
      <h2>Checkout</h2>

      {/* Cart Summary */}
      <div className="section">
        <h3>Your Order</h3>
        {cart.length === 0 ? (
          <p>Your cart is empty</p>
        ) : (
          <>
            <ul className="cart-items">
              {cart.map((item) => (
                <li key={item.cartItemId}>
                  {item.menuItemName} - {item.quantity} × ₹{item.priceAtPurchase}
                  <span>₹{item.quantity * item.priceAtPurchase}</span>
                </li>
              ))}
            </ul>
            <div className="cart-total">
              <strong>Total: <span>₹{cartTotal}</span></strong>
            </div>
          </>
        )}
      </div>

      {/* Address Selection */}
      <div className="section">
        <h3>Delivery Address</h3>
        {addresses.length > 0 ? (
          <div className="address-list">
            {addresses.map((addr) => (
              <div 
                key={addr.addressId} 
                className={`address-card ${selectedAddress === addr.addressId ? 'selected' : ''}`}
                onClick={() => setSelectedAddress(addr.addressId)}
              >
                <input
                  type="radio"
                  name="address"
                  checked={selectedAddress === addr.addressId}
                  onChange={() => {}}
                />
                <div>
                  <strong>{addr.label}</strong>
                  <p>{addr.street}, {addr.city} - {addr.pincode}</p>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p>No saved addresses found</p>
        )}

        <h4>Add New Address</h4>
        <div className="address-form">
          <input
            placeholder="Label (Home, Work, etc.)"
            value={newAddress.label}
            onChange={(e) => setNewAddress({ ...newAddress, label: e.target.value })}
          />
          <input
            placeholder="Street Address"
            value={newAddress.street}
            onChange={(e) => setNewAddress({ ...newAddress, street: e.target.value })}
          />
          <input
            placeholder="City"
            value={newAddress.city}
            onChange={(e) => setNewAddress({ ...newAddress, city: e.target.value })}
          />
          <input
            placeholder="Pincode"
            value={newAddress.pincode}
            onChange={(e) => setNewAddress({ ...newAddress, pincode: e.target.value })}
          />
          <button 
            onClick={handleAddAddress}
            // disabled={!newAddress.label || !newAddress.street || !newAddress.city || !newAddress.pincode}
          >
            Save Address
          </button>
        </div>
      </div>

      {/* Payment Method */}
      <div className="section">
        <h3>Payment Method</h3>
        <div className="payment-methods">
          <label className={paymentMethod === "UPI" ? "selected" : ""}>
            <input
              type="radio"
              name="payment"
              value="UPI"
              checked={paymentMethod === "UPI"}
              onChange={() => setPaymentMethod("UPI")}
            />
            UPI
          </label>
          
          <label className={paymentMethod === "Card" ? "selected" : ""}>
            <input
              type="radio"
              name="payment"
              value="Card"
              checked={paymentMethod === "Card"}
              onChange={() => setPaymentMethod("Card")}
            />
            Credit/Debit Card
          </label>
          
          <label className={paymentMethod === "Cash on Delivery" ? "selected" : ""}>
            <input
              type="radio"
              name="payment"
              value="Cash on Delivery"
              checked={paymentMethod === "Cash on Delivery"}
              onChange={() => setPaymentMethod("Cash on Delivery")}
            />
            Cash on Delivery
          </label>
        </div>

        {paymentMethod === "UPI" && (
          <div className="payment-details upi">
            <img src={qrCode} alt="UPI QR Code" />
            <p>Scan this QR code to pay via any UPI app</p>
            <p className="upi-id">UPI ID: hotpot@upi</p>
          </div>
        )}

        {paymentMethod === "Card" && (
          <div className="payment-details card">
            <input type="text" placeholder="Card Number" maxLength={16} />
            <input type="text" placeholder="Cardholder Name" />
            <div className="card-row">
              <input type="text" placeholder="MM/YY" maxLength={5} />
              <input type="text" placeholder="CVV" maxLength={3} />
            </div>
          </div>
        )}

        {paymentMethod === "Cash on Delivery" && (
          <div className="payment-details cod">
            <p>Pay in cash when your order is delivered</p>
          </div>
        )}
      </div>

      {/* Payment Button */}
      <div className="checkout-actions">
        <button
          className="pay-button"
          onClick={processPayment}
          disabled={cart.length === 0 || !selectedAddress || paymentStatus === "processing"}
        >
          {paymentStatus === "processing" ? (
            <span className="processing">
              <span className="spinner"></span> Processing...
            </span>
          ) : (
            `Pay ₹${cartTotal}`
          )}
        </button>
        
        {paymentStatus === "failed" && (
          <p className="error-message">
            Payment failed. Please try again or use a different payment method.
          </p>
        )}
      </div>
    </div>
  );
};

export default CheckoutPage;