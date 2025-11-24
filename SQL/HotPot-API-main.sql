select * from Admins
-- DBCC CHECKIDENT ('Table_Name', RESEED, 0); ---- (t is used to rest the table id to start from 1)

SELECT * FROM MealTypes;
SELECT * FROM Cuisines;
SELECT * FROM MenuItems;
SELECT * FROM Restaurants;

--Update MenuItems
--set IsAvailable = 1;


SELECT * FROM DeliveryPartners; 

--update DeliveryPartners
--set IsAvailable = 1;


--where DeliveryPartnerId =5;


SELECT * FROM Customers;
SELECT * FROM Carts;
SELECT * FROM CartItems;

SELECT * FROM Users;
SELECT * FROM Admins;
SELECT * FROM Customers;
SELECT * FROM CustomerAddresses;
SELECT * FROM RestaurantManagers;
SELECT * FROM Restaurants;
SELECT * FROM DeliveryPartners; 

--DELETE FROM OrderItems;
DBCC CHECKIDENT ('OrderItems', RESEED, 4);

--DELETE FROM Orders;
DBCC CHECKIDENT ('Orders', RESEED, 0);

--DELETE FROM Carts;
--DBCC CHECKIDENT ('Carts', RESEED, 0);

--DELETE FROM CartItems;
--DBCC CHECKIDENT ('CartItems', RESEED, 0);

--UPDATE Orders
--SET 
--    DeliveryPartnerId = NULL,
--    OrderStatusId = 1
--WHERE OrderId = 7;


SELECT * FROM DeliveryPartners; 

SELECT * FROM Orders; 
SELECT * FROM OrderItems;

SELECT * FROM OrderStatuses;
SELECT * FROM PaymentMethods;
SELECT * FROM PaymentStatuses;


SELECT 
    o.OrderId, 
	oi.OrderItemId,
    o.CustomerId, 
    oi.MenuItemId,
	mi.Name,
    oi.quantity,
	mi.RestaurantId,
	o.OrderStatusId
FROM orders o
JOIN OrderItems oi ON o.OrderId = oi.OrderId
JOIN MenuItems mi ON oi.MenuItemId = mi.Id
WHERE mi.RestaurantId = 7;


--- ctrl + k & ctrl + c (comment)
--- ctrl + k & ctrl + u (uncomment)

UPDATE DeliveryPartners
SET IsAvailable = 1
where DeliveryPartnerId = 1004;


--UPDATE Orders
--SET OrderStatusId = 4
--where OrderId = 5;
--UPDATE DeliveryPartners
--SET IsAvailable = 0
--where DeliveryPartnerId = 1;


--UPDATE Restaurants
--SET Restaurantlogo = 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTVjW_BnwlRWaYtNlHDI6fKfLUNgPblGZeSLQ&s'
--where RestaurantId = 1;

--INSERT INTO RESTAURANTS (RestaurantName, Location, ContactNumber, Email, RestaurantLogo) VALUES
--('Snackers', '21G, Anna nagar, Chennai', '9876543210', 'snackers@gmail.com', 'snackers_logo.png'),
--('Planet-B', '99, TNagar, Chennai', '9123456780', 'planetb@gmail.com', 'planetb_logo.png'),
--('KFC', 'Bharathi Street, Chennai', '9012345678', 'kfc@gmail.com', 'kfc_logo.png'),
--('Burger King', '78/3, Raja Street, Chennai', '9988776655', 'burgerking@gmail.com', 'bk_logo.png'),
--('Amman Mess', '11, Siruseri, Chennai', '9876501234', 'ammanmess@gmail.com', 'amman_logo.png'),
--('Thalappakatti', '123, Siruseri, Chennai', '9123467890', 'thalappakatti@gmail.com', 'thalappakatti_logo.png'),
--('ibaco', '10, Ramapuram, Chennai', '9090909090', 'ibaco@gmail.com', 'https://i.ytimg.com/vi/Rpfzp0Wlzt0/maxresdefault.jpg');

SELECT * FROM Restaurants

INSERT INTO OrderStatuses (StatusName) VALUES
('Order Placed'),
('Order Confirmed'),
('Preparing'),
('Ready for Pick Up'),
('Out for Delivery'),
('Delivered'),
('Cancelled');


INSERT INTO PaymentMethods (MethodName) VALUES
('UPI'),
('Cash on Delivery'),
('Card');


INSERT INTO PaymentStatuses (StatusName) VALUES
('Pending'),
('Success'),
('Failed'),
('Refunded'),
('Cancelled');


INSERT INTO MealTypes (Name) VALUES 
('Vegetarian'),
('Non-Vegetarian'),
('Vegan'),
('Eggetarian'),
('Jain'),
('Gluten-Free');

INSERT INTO Cuisines (Name) VALUES 
('South Indian'),
('North Indian'),
('Tandoori'),
('Biryani'),
('Parotta'),
('Pizza'),
('Burger'),
('Rolls'),
('Shawarma'),
('Momos'),
('Kebab'),
('Noodles'),
('Pasta'),
('Cake'),
('Ice Cream'),
('Seafood');


select * from Cuisines
SELECT * FROM MenuItems;

-- Inserting MenuItems

-- Restaurant 1: Snackers

--INSERT INTO MenuItems (
--  name, description, cookingTime, price, imagePath, availabilityTime, isAvailable,
--  cuisineId, mealTypeId, restaurantId, calories, proteins, fats, carbohydrates, tasteInfo
--) VALUES
--('Cheese Burst Burger', 'Juicy patty with molten cheese.', '00:10:00', 120, '/images/burger1.jpg', '11:00 AM - 09:00 PM', 'true', 5, 1, 1, 480, 18, 25, 45, 'Spicy & Cheesy'),
--('Classic Veg Roll', 'Spicy vegetables wrapped in a soft paratha.', '00:12:00', 100, '/images/veg_roll1.jpg', '11:00 AM - 09:00 PM', 'true', 1, 1, 1, 350, 8, 15, 40, 'Spicy'),
--('Grilled Chicken Shawarma', 'Grilled chicken wrapped in pita bread with tangy sauce.', '00:15:00', 180, '/images/chicken_shawarma.jpg', '11:00 AM - 09:00 PM', 'true', 9, 2, 1, 450, 25, 20, 50, 'Savory'),
--('Paneer Tikka', 'Grilled paneer cubes marinated with spices.', '00:20:00', 150, '/images/paneer_tikka.jpg', '11:00 AM - 09:00 PM', 'true', 2, 1, 1, 250, 20, 10, 20, 'Smoky'),
--('Steamed Veg Momos', 'Soft dumplings stuffed with vegetables.', '00:10:00', 100, '/images/veg_momos.jpg', '11:00 AM - 09:00 PM', 'true', 10, 1, 1, 200, 7, 6, 30, 'Mild'),
--('Crispy Chicken Wings', 'Crispy fried wings served with a spicy dip.', '00:15:00', 220, '/images/chicken_wings.jpg', '11:00 AM - 09:00 PM', 'true', 9, 2, 1, 500, 30, 22, 25, 'Spicy'),
--('Cheesy Veg Pizza', 'A cheesy pizza topped with mixed veggies.', '00:25:00', 250, '/images/veg_pizza.jpg', '11:00 AM - 09:00 PM', 'true', 6, 1, 1, 350, 12, 18, 40, 'Cheesy'),
--('Aromatic Chicken Biryani', 'Flavorful rice cooked with chicken and spices.', '00:40:00', 300, '/images/chicken_biryani.jpg', '11:00 AM - 09:00 PM', 'true', 4, 2, 1, 600, 35, 25, 55, 'Flavorful'),
--('Crispy Veg Pakora', 'Deep-fried fritters made from gram flour and vegetables.', '00:10:00', 120, '/images/veg_pakora.jpg', '11:00 AM - 09:00 PM', 'true', 1, 1, 1, 250, 8, 15, 30, 'Crispy'),
--('Grilled Chicken Kebab', 'Minced chicken skewers grilled to perfection.', '00:20:00', 180, '/images/chicken_kebab.jpg', '11:00 AM - 09:00 PM', 'true', 11, 2, 1, 350, 25, 18, 30, 'Smoky'),
--('Veg Sandwich', 'Fresh multigrain sandwich filled with veggies.', '00:08:00', 90, '/images/veg_sandwich.jpg', '11:00 AM - 09:00 PM', 'true', 1, 1, 1, 180, 5, 4, 25, 'Fresh'),
--('Chicken Caesar Wrap', 'Grilled chicken with creamy Caesar dressing wrapped in a tortilla.', '00:20:00', 180, '/images/chicken_caesar_wrap.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 1, 350, 30, 15, 40, 'Creamy'),
--('Veg Noodles', 'Stir-fried noodles with assorted vegetables.', '00:12:00', 130, '/images/veg_noodles.jpg', '11:00 AM - 09:00 PM', 'true', 12, 1, 1, 220, 8, 7, 45, 'Mild'),
--('Chicken Burger', 'Grilled chicken patty with lettuce and mayo in a bun.', '00:10:00', 150, '/images/chicken_burger.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 1, 350, 30, 15, 35, 'Savory'),
--('Vegetable Samosa', 'Golden fried pastry with spiced potato filling.', '00:08:00', 80, '/images/veg_samosa.jpg', '11:00 AM - 09:00 PM', 'true', 10, 1, 1, 150, 5, 8, 18, 'Crispy'),
--('Paneer Makhani', 'Paneer cubes in a creamy tomato gravy.', '00:25:00', 180, '/images/paneer_makhani.jpg', '11:00 AM - 09:00 PM', 'true', 2, 1, 1, 500, 22, 30, 45, 'Creamy & Tangy'),
--('Pasta Primavera', 'Pasta with fresh vegetables in a light sauce.', '00:15:00', 220, '/images/pasta_primavera.jpg', '11:00 AM - 09:00 PM', 'true', 13, 1, 1, 300, 12, 18, 40, 'Cheesy'),
--('BBQ Chicken Pizza', 'Pizza with BBQ chicken, cheese, and onions.', '00:30:00', 300, '/images/bbq_chicken_pizza.jpg', '11:00 AM - 09:00 PM', 'true', 6, 2, 1, 450, 28, 25, 40, 'Smoky & Sweet'),
--('Veg Tacos', 'Crispy taco shells filled with seasoned vegetables.', '00:12:00', 140, '/images/veg_tacos.jpg', '11:00 AM - 09:00 PM', 'true', 8, 1, 1, 200, 5, 8, 30, 'Tangy'),
--('Kebabs with Roti', 'Grilled kebabs served with soft roti.', '00:20:00', 200, '/images/kebabs_roti.jpg', '11:00 AM - 09:00 PM', 'true', 11, 2, 1, 400, 28, 18, 40, 'Spicy'),
--('Fish Fry', 'Crispy fried fish fillets served with a side of salad.', '00:15:00', 250, '/images/fish_fry.jpg', '11:00 AM - 09:00 PM', 'true', 16, 2, 1, 350, 25, 20, 30, 'Crispy');

-- Restaurant 2: Planet-B

--INSERT INTO MenuItems (
--  name, description, cookingTime, price, imagePath, availabilityTime, isAvailable,
--  cuisineId, mealTypeId, restaurantId, calories, proteins, fats, carbohydrates, tasteInfo
--) VALUES
--('Veg Cheese Burger', 'Crispy burger with fresh veggies and cheese.', '00:10:00', 120, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 7, 1, 2, 400, 15, 20, 40, 'Cheesy & Savory'),
--('Chicken Shawarma Roll', 'Soft pita bread with juicy chicken and tahini sauce.', '00:15:00', 180, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 9, 2, 2, 450, 30, 18, 40, 'Tangy & Smoky'),
--('Tandoori Paneer Tikka', 'Grilled paneer cubes with a rich blend of spices.', '00:20:00', 150, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 3, 1, 2, 250, 18, 12, 20, 'Smoky'),
--('Veg Sizzling Noodles', 'Sizzling plate of noodles with mixed veggies.', '00:12:00', 120, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 12, 1, 2, 300, 10, 8, 45, 'Spicy & Savory'),
--('Chicken Kebab', 'Grilled chicken skewers marinated in spices.', '00:20:00', 180, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 11, 2, 2, 350, 30, 15, 35, 'Smoky'),
--('Paneer Butter Masala', 'Soft paneer cubes in rich, creamy tomato gravy.', '00:25:00', 200, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 2, 1, 2, 450, 20, 25, 40, 'Rich & Creamy'),
--('Veg Biryani', 'Aromatic rice with vegetables and a blend of spices.', '00:30:00', 250, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 4, 1, 2, 500, 12, 18, 60, 'Flavorful & Spicy'),
--('Spicy Veg Tacos', 'Taco shells filled with spicy veggies and cheese.', '00:10:00', 140, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 8, 1, 2, 200, 5, 7, 30, 'Spicy'),
--('Chicken Caesar Salad', 'Grilled chicken served with Caesar dressing and veggies.', '00:20:00', 180, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 6, 2, 2, 300, 35, 10, 15, 'Creamy & Savory'),
--('Veg Pakoras', 'Crispy fried fritters with mixed vegetables and gram flour.', '00:08:00', 100, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 1, 1, 2, 200, 6, 10, 25, 'Crunchy'),
--('Pasta Arrabiata', 'Pasta in a spicy tomato sauce with garlic and chili.', '00:15:00', 220, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 13, 1, 2, 350, 12, 18, 45, 'Spicy & Tangy'),
--('Cheese Veg Pizza', 'Pizza topped with cheese, veggies, and a thin crust.', '00:25:00', 250, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 6, 1, 2, 400, 15, 20, 50, 'Cheesy'),
--('Egg Fried Rice', 'Fried rice with scrambled eggs and vegetables.', '00:12:00', 130, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 12, 4, 2, 320, 15, 14, 45, 'Savory'),
--('Mushroom Risotto', 'Creamy risotto with fresh mushrooms and Parmesan cheese.', '00:20:00', 220, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 12, 1, 2, 350, 10, 12, 40, 'Creamy & Rich'),
--('Chicken Parmesan', 'Breaded chicken cutlets topped with marinara sauce and cheese.', '00:25:00', 250, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 6, 2, 2, 480, 35, 20, 40, 'Savory & Cheesy'),
--('Vegan Burger', 'A delicious plant-based patty served with veggies.', '00:10:00', 130, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 1, 3, 2, 350, 12, 10, 50, 'Vegan & Hearty'),
--('Veg Hakka Noodles', 'Stir-fried noodles with mixed vegetables.', '00:15:00', 140, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 12, 1, 2, 280, 8, 9, 50, 'Mild & Savory'),
--('Chicken Wrap', 'Tender chicken wrapped in a soft tortilla with fresh veggies.', '00:10:00', 150, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 7, 2, 2, 400, 30, 12, 50, 'Tangy & Savory'),
--('Veg Soup', 'Healthy and warm vegetable soup with a variety of vegetables.', '00:10:00', 100, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 1, 1, 2, 150, 5, 3, 20, 'Light & Hearty'),
--('Paneer Makhani with Naan', 'Paneer in creamy tomato sauce served with naan.', '00:25:00', 220, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 2, 1, 2, 550, 25, 28, 45, 'Rich & Creamy'),
--('Beef Burger', 'Grilled beef patty with cheese, lettuce, and sauce.', '00:10:00', 170, 'https://images.unsplash.com/photo-1605295096661-0bbd45cb8c8d', '11:00 AM - 09:00 PM', 'true', 7, 2, 2, 500, 35, 30, 45, 'Savory & Hearty');

-- Restaurant 3: KFC

--INSERT INTO MenuItems (
--  name, description, cookingTime, price, imagePath, availabilityTime, isAvailable,
--  cuisineId, mealTypeId, restaurantId, calories, proteins, fats, carbohydrates, tasteInfo
--) VALUES
--('Zinger Burger', 'Crispy fried chicken fillet with lettuce and mayo in a bun.', '00:10:00', 180, '/images/zinger_burger.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 3, 500, 30, 22, 50, 'Crispy & Savory'),
--('Fried Chicken Bucket', 'Bucket of crispy fried chicken pieces, served with dipping sauce.', '00:20:00', 600, '/images/fried_chicken_bucket.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 3, 800, 40, 45, 50, 'Crispy & Savory'),
--('Chicken Popcorn', 'Small crispy chicken bites, served with dipping sauce.', '00:05:00', 120, '/images/chicken_popcorn.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 3, 350, 18, 20, 30, 'Crunchy & Savory'),
--('Spicy Chicken Wings', 'Hot and spicy chicken wings, served with a side of ranch.', '00:10:00', 180, '/images/spicy_chicken_wings.jpg', '11:00 AM - 09:00 PM', 'true', 9, 2, 3, 400, 25, 22, 40, 'Spicy & Tangy'),
--('Classic Chicken Burger', 'Classic fried chicken burger with pickles and mayo.', '00:10:00', 160, '/images/classic_chicken_burger.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 3, 480, 28, 25, 40, 'Crispy & Savory'),
--('Veg Zinger Burger', 'Crispy veggie patty with lettuce and mayo in a soft bun.', '00:10:00', 140, '/images/veg_zinger_burger.jpg', '11:00 AM - 09:00 PM', 'true', 5, 1, 3, 400, 12, 15, 45, 'Crispy & Vegetarian'),
--('KFC Popcorn Shrimp', 'Crispy shrimp bites served with a spicy dipping sauce.', '00:05:00', 200, '/images/popcorn_shrimp.jpg', '11:00 AM - 09:00 PM', 'true', 16, 2, 3, 320, 25, 15, 30, 'Crunchy & Savory'),
--('Chicken Tenders', 'Breaded chicken tenders served with your choice of sauce.', '00:15:00', 200, '/images/chicken_tenders.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 3, 400, 30, 18, 45, 'Crispy & Savory'),
--('Gravy Fries', 'Crispy fries topped with rich brown gravy.', '00:08:00', 100, '/images/gravy_fries.jpg', '11:00 AM - 09:00 PM', 'true', 5, 1, 3, 250, 5, 12, 35, 'Savory & Hearty'),
--('BBQ Chicken', 'Juicy chicken pieces coated in a tangy BBQ sauce.', '00:20:00', 250, '/images/bbq_chicken.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 3, 600, 35, 30, 40, 'Smoky & Tangy'),
--('Veg Caesar Salad', 'Fresh lettuce, tomatoes, and croutons topped with Caesar dressing.', '00:10:00', 150, '/images/veg_caesar_salad.jpg', '11:00 AM - 09:00 PM', 'true', 1, 1, 3, 200, 5, 10, 30, 'Fresh & Creamy'),
--('Mashed Potatoes', 'Creamy mashed potatoes with gravy.', '00:10:00', 120, '/images/mashed_potatoes.jpg', '11:00 AM - 09:00 PM', 'true', 5, 1, 3, 220, 4, 12, 35, 'Creamy & Savory'),
--('Hot Wings', 'Delicious hot wings served with a side of blue cheese dressing.', '00:10:00', 150, '/images/hot_wings.jpg', '11:00 AM - 09:00 PM', 'true', 9, 2, 3, 350, 20, 18, 40, 'Spicy & Tangy'),
--('Coleslaw', 'Fresh shredded cabbage salad with a creamy dressing.', '00:08:00', 80, '/images/coleslaw.jpg', '11:00 AM - 09:00 PM', 'true', 1, 1, 3, 100, 2, 8, 15, 'Creamy & Light'),
--('Chicken Sandwich', 'Fried chicken breast, lettuce, and mayo on a soft bun.', '00:10:00', 150, '/images/chicken_sandwich.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 3, 450, 28, 20, 40, 'Crispy & Savory'),
--('Spicy Veg Wrap', 'Spicy veggie filling wrapped in a soft tortilla with sauce.', '00:15:00', 130, '/images/spicy_veg_wrap.jpg', '11:00 AM - 09:00 PM', 'true', 5, 1, 3, 350, 8, 10, 45, 'Spicy & Savory'),
--('Pineapple Fritters', 'Crispy fried pineapple rings served with syrup.', '00:08:00', 110, '/images/pineapple_fritters.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 3, 200, 2, 10, 45, 'Sweet & Crispy'),
--('Veg Wrap', 'Grilled vegetables and sauces wrapped in a tortilla.', '00:15:00', 130, '/images/veg_wrap.jpg', '11:00 AM - 09:00 PM', 'true', 5, 1, 3, 300, 6, 8, 35, 'Savory & Light'),
--('Chicken Rice Bowl', 'Grilled chicken pieces served on a bed of rice.', '00:20:00', 220, '/images/chicken_rice_bowl.jpg', '11:00 AM - 09:00 PM', 'true', 12, 2, 3, 500, 35, 18, 45, 'Savory & Hearty'),
--('Tandoori Chicken', 'Traditional tandoori chicken marinated with spices and grilled.', '00:25:00', 300, '/images/tandoori_chicken.jpg', '11:00 AM - 09:00 PM', 'true', 3, 2, 3, 550, 40, 25, 45, 'Smoky & Spicy');

-- Restaurant 4: Burger King

--INSERT INTO MenuItems (
--  name, description, cookingTime, price, imagePath, availabilityTime, isAvailable,
--  cuisineId, mealTypeId, restaurantId, calories, proteins, fats, carbohydrates, tasteInfo
--) VALUES
--('Whopper', 'Classic flame-grilled beef patty with fresh lettuce, tomatoes, and mayo.', '00:15:00', 220, '/images/whopper.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 4, 700, 40, 35, 50, 'Savory & Hearty'),
--('Chicken Fries', 'Crispy chicken-shaped fries, served with a dipping sauce.', '00:10:00', 120, '/images/chicken_fries.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 4, 400, 22, 20, 30, 'Crispy & Savory'),
--('Cheese Whopper', 'Whopper with a melted layer of cheese, fresh vegetables, and sauce.', '00:15:00', 250, '/images/cheese_whopper.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 4, 800, 40, 40, 55, 'Cheesy & Hearty'),
--('Chicken Tendercrisp', 'Crispy breaded chicken fillet with lettuce, tomatoes, and mayo.', '00:15:00', 190, '/images/chicken_tendercrisp.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 4, 600, 30, 25, 40, 'Crispy & Savory'),
--('Veggie Burger', 'Grilled veggie patty with lettuce, tomatoes, pickles, and mayo.', '00:10:00', 150, '/images/veggie_burger.jpg', '11:00 AM - 09:00 PM', 'true', 5, 1, 4, 350, 10, 12, 45, 'Vegetarian & Light'),
--('Double Cheeseburger', 'Double beef patty with melted cheese, pickles, and ketchup.', '00:10:00', 180, '/images/double_cheeseburger.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 4, 650, 40, 35, 50, 'Cheesy & Hearty'),
--('Onion Rings', 'Crispy and crunchy onion rings, served with dipping sauce.', '00:10:00', 100, '/images/onion_rings.jpg', '11:00 AM - 09:00 PM', 'true', 5, 1, 4, 300, 5, 15, 40, 'Crunchy & Savory'),
--('Chicken Nugget Meal', 'Crispy chicken nuggets served with fries and a dipping sauce.', '00:15:00', 220, '/images/chicken_nugget_meal.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 4, 500, 30, 25, 45, 'Crispy & Hearty'),
--('Veggie Wrap', 'Grilled vegetables wrapped in a soft tortilla with a tangy sauce.', '00:10:00', 120, '/images/veggie_wrap.jpg', '11:00 AM - 09:00 PM', 'true', 5, 1, 4, 350, 8, 10, 50, 'Savory & Light'),
--('Chicken Royale', 'Crispy fried chicken fillet with fresh veggies and mayo in a soft bun.', '00:12:00', 190, '/images/chicken_royale.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 4, 600, 32, 30, 50, 'Crispy & Savory'),
--('Fish Sandwich', 'Breaded fish fillet with tartar sauce, lettuce, and pickles.', '00:15:00', 180, '/images/fish_sandwich.jpg', '11:00 AM - 09:00 PM', 'true', 16, 2, 4, 400, 25, 18, 40, 'Fish & Savory'),
--('Cheesy Fries', 'Crispy fries topped with melted cheese and jalapenos.', '00:08:00', 140, '/images/cheesy_fries.jpg', '11:00 AM - 09:00 PM', 'true', 5, 1, 4, 350, 5, 20, 40, 'Cheesy & Spicy'),
--('BBQ Bacon King', 'Flame-grilled beef patty with BBQ sauce, bacon, and cheese.', '00:15:00', 300, '/images/bbq_bacon_king.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 4, 900, 50, 45, 60, 'Smoky & Savory'),
--('Crispy Chicken Salad', 'Fresh veggies topped with crispy chicken and ranch dressing.', '00:15:00', 200, '/images/crispy_chicken_salad.jpg', '11:00 AM - 09:00 PM', 'true', 6, 2, 4, 450, 30, 25, 35, 'Creamy & Crunchy'),
--('Poutine', 'Fries topped with cheese curds and gravy.', '00:10:00', 160, '/images/poutine.jpg', '11:00 AM - 09:00 PM', 'true', 5, 1, 4, 350, 5, 20, 40, 'Savory & Hearty'),
--('Chicken Caesar Salad', 'Grilled chicken with romaine lettuce, croutons, and Caesar dressing.', '00:15:00', 200, '/images/chicken_caesar_salad.jpg', '11:00 AM - 09:00 PM', 'true', 6, 2, 4, 350, 30, 15, 35, 'Savory & Creamy'),
--('Cheese Sticks', 'Crispy breaded cheese sticks served with marinara sauce.', '00:10:00', 120, '/images/cheese_sticks.jpg', '11:00 AM - 09:00 PM', 'true', 6, 1, 4, 350, 12, 18, 30, 'Cheesy & Savory'),
--('Mushroom Swiss Burger', 'Beef patty with melted Swiss cheese and grilled mushrooms.', '00:15:00', 220, '/images/mushroom_swiss_burger.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 4, 650, 35, 30, 45, 'Savory & Hearty'),
--('Spicy Chicken Wrap', 'Spicy crispy chicken pieces wrapped in a tortilla with creamy sauce.', '00:10:00', 150, '/images/spicy_chicken_wrap.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 4, 400, 25, 18, 40, 'Spicy & Savory'),
--('Frozen Fanta', 'Frozen Fanta with crushed ice and a sweet, tangy flavor.', '00:05:00', 100, '/images/frozen_fanta.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 4, 150, 0, 0, 40, 'Sweet & Tangy');

-- Restaurant 5: Amman Mess

--INSERT INTO MenuItems (
--  name, description, cookingTime, price, imagePath, availabilityTime, isAvailable,
--  cuisineId, mealTypeId, restaurantId, calories, proteins, fats, carbohydrates, tasteInfo
--) VALUES
--('Mutton Biryani', 'Aromatic mutton and rice dish cooked with a blend of spices.', '00:40:00', 350, '/images/mutton_biryani.jpg', '11:00 AM - 09:00 PM', 'true', 4, 2, 5, 700, 40, 30, 65, 'Spicy & Flavorful'),
--('Chicken Chettinad', 'Spicy chicken cooked with a traditional Chettinad masala.', '00:35:00', 280, '/images/chicken_chettinad.jpg', '11:00 AM - 09:00 PM', 'true', 2, 2, 5, 550, 35, 25, 45, 'Spicy & Tangy'),
--('Parotta & Korma', 'Flaky parotta served with rich chicken korma.', '00:20:00', 180, '/images/parotta_korma.jpg', '11:00 AM - 09:00 PM', 'true', 5, 2, 5, 650, 30, 40, 50, 'Rich & Creamy'),
--('Fish Curry', 'Spicy fish curry cooked with fresh coconut and tamarind.', '00:30:00', 250, '/images/fish_curry.jpg', '11:00 AM - 09:00 PM', 'true', 16, 2, 5, 400, 25, 18, 35, 'Spicy & Tangy'),
--('Veg Thali', 'A wholesome thali with a variety of vegetarian curries and rice.', '00:25:00', 220, '/images/veg_thali.jpg', '11:00 AM - 09:00 PM', 'true', 1, 1, 5, 500, 12, 20, 60, 'Light & Savory'),
--('Mutton Sukka', 'Tender mutton pieces dry-cooked with aromatic spices.', '00:35:00', 280, '/images/mutton_sukka.jpg', '11:00 AM - 09:00 PM', 'true', 4, 2, 5, 600, 40, 30, 40, 'Savory & Spicy'),
--('Chicken Biryani', 'Aromatic chicken and rice dish cooked with spices and herbs.', '00:40:00', 300, '/images/chicken_biryani.jpg', '11:00 AM - 09:00 PM', 'true', 4, 2, 5, 650, 40, 25, 55, 'Spicy & Flavorful'),
--('Kothu Parotta', 'Chopped parotta stir-fried with chicken and spices.', '00:20:00', 250, '/images/kothu_parotta.jpg', '11:00 AM - 09:00 PM', 'true', 5, 2, 5, 700, 35, 30, 60, 'Spicy & Hearty'),
--('Veg Kurma', 'A rich and creamy curry made with mixed vegetables and spices.', '00:25:00', 180, '/images/veg_kurma.jpg', '11:00 AM - 09:00 PM', 'true', 2, 1, 5, 500, 10, 25, 55, 'Creamy & Mild'),
--('Prawn Masala', 'Prawns cooked in a flavorful, spicy masala with coconut.', '00:30:00', 350, '/images/prawn_masala.jpg', '11:00 AM - 09:00 PM', 'true', 16, 2, 5, 500, 40, 20, 45, 'Spicy & Rich'),
--('Chicken 65', 'Crispy fried chicken marinated with a blend of spices.', '00:15:00', 180, '/images/chicken_65.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 5, 400, 30, 25, 40, 'Spicy & Crispy'),
--('Mutton Shorba', 'Spicy mutton broth with traditional herbs and spices.', '00:25:00', 220, '/images/mutton_shorba.jpg', '11:00 AM - 09:00 PM', 'true', 4, 2, 5, 350, 25, 15, 30, 'Savory & Spicy'),
--('Naan & Butter Chicken', 'Soft naan served with creamy butter chicken curry.', '00:20:00', 220, '/images/naan_butter_chicken.jpg', '11:00 AM - 09:00 PM', 'true', 2, 2, 5, 650, 40, 35, 50, 'Creamy & Hearty'),
--('Vegetable Pulao', 'Fluffy rice cooked with mixed vegetables and aromatic spices.', '00:20:00', 200, '/images/vegetable_pulao.jpg', '11:00 AM - 09:00 PM', 'true', 4, 1, 5, 500, 8, 15, 70, 'Mild & Flavorful'),
--('Fish Fry', 'Crispy fried fish fillet coated with a blend of spices.', '00:20:00', 230, '/images/fish_fry.jpg', '11:00 AM - 09:00 PM', 'true', 16, 2, 5, 450, 35, 20, 30, 'Crispy & Spicy'),
--('Paneer Butter Masala', 'Paneer cubes in a rich, creamy tomato gravy.', '00:30:00', 250, '/images/paneer_butter_masala.jpg', '11:00 AM - 09:00 PM', 'true', 2, 1, 5, 500, 18, 22, 45, 'Rich & Creamy'),
--('Egg Biryani', 'Fragrant biryani made with eggs and a blend of spices.', '00:40:00', 250, '/images/egg_biryani.jpg', '11:00 AM - 09:00 PM', 'true', 4, 4, 5, 550, 18, 20, 60, 'Spicy & Flavorful'),
--('Mutton Korma', 'Tender mutton cooked in a rich, creamy sauce.', '00:35:00', 320, '/images/mutton_korma.jpg', '11:00 AM - 09:00 PM', 'true', 4, 2, 5, 650, 40, 30, 50, 'Rich & Hearty'),
--('Fried Idli', 'Crispy fried idlis served with chutneys.', '00:12:00', 120, '/images/fried_idli.jpg', '11:00 AM - 09:00 PM', 'true', 1, 1, 5, 250, 6, 10, 30, 'Crispy & Savory'),
--('Malai Kofta', 'Vegetable koftas in a rich and creamy gravy.', '00:30:00', 250, '/images/malai_kofta.jpg', '11:00 AM - 09:00 PM', 'true', 2, 1, 5, 500, 12, 25, 60, 'Creamy & Rich');

-- Restaurant 6: Thalappakatti

--INSERT INTO MenuItems (
--  name, description, cookingTime, price, imagePath, availabilityTime, isAvailable,
--  cuisineId, mealTypeId, restaurantId, calories, proteins, fats, carbohydrates, tasteInfo
--) VALUES
--('Thalappakatti Mutton Biryani', 'Aromatic mutton biryani made with long-grain basmati rice and tender mutton.', '00:45:00', 350, '/images/thalappakatti_mutton_biryani.jpg', '11:00 AM - 09:00 PM', 'true', 4, 2, 6, 700, 45, 35, 70, 'Spicy & Flavorful'),
--('Chicken Chettinad Biryani', 'Chicken cooked in spicy Chettinad masala and layered with fragrant rice.', '00:45:00', 320, '/images/chicken_chettinad_biryani.jpg', '11:00 AM - 09:00 PM', 'true', 4, 2, 6, 650, 40, 30, 65, 'Spicy & Rich'),
--('Mutton Korma', 'Slow-cooked mutton in a creamy, aromatic korma gravy.', '00:40:00', 300, '/images/mutton_korma.jpg', '11:00 AM - 09:00 PM', 'true', 4, 2, 6, 650, 45, 25, 50, 'Rich & Creamy'),
--('Kothu Parotta', 'Chopped parotta stir-fried with spicy chicken, eggs, and masalas.', '00:30:00', 250, '/images/kothu_parotta.jpg', '11:00 AM - 09:00 PM', 'true', 5, 2, 6, 650, 30, 35, 60, 'Spicy & Hearty'),
--('Fish Curry', 'Fresh fish cooked in a tangy, spicy curry with tamarind and coconut milk.', '00:35:00', 280, '/images/fish_curry.jpg', '11:00 AM - 09:00 PM', 'true', 16, 2, 6, 450, 30, 20, 40, 'Tangy & Spicy'),
--('Vegetable Biryani', 'Fragrant biryani made with a medley of vegetables and aromatic spices.', '00:45:00', 220, '/images/veg_biryani.jpg', '11:00 AM - 09:00 PM', 'true', 4, 1, 6, 500, 8, 18, 70, 'Mild & Flavorful'),
--('Mutton Sukka', 'Dry-cooked mutton with a blend of spices, served with parotta.', '00:30:00', 300, '/images/mutton_sukka.jpg', '11:00 AM - 09:00 PM', 'true', 4, 2, 6, 550, 45, 35, 45, 'Savory & Spicy'),
--('Egg Kothu Parotta', 'Chopped parotta stir-fried with scrambled eggs and spicy masalas.', '00:25:00', 220, '/images/egg_kothu_parotta.jpg', '11:00 AM - 09:00 PM', 'true', 5, 4, 6, 500, 20, 18, 60, 'Mild & Hearty'),
--('Paneer Butter Masala', 'Soft paneer cooked in a rich, creamy tomato gravy.', '00:35:00', 240, '/images/paneer_butter_masala.jpg', '11:00 AM - 09:00 PM', 'true', 2, 1, 6, 500, 15, 25, 55, 'Rich & Creamy'),
--('Mutton Shorba', 'Spicy mutton broth with fresh herbs and a touch of tamarind.', '00:25:00', 230, '/images/mutton_shorba.jpg', '11:00 AM - 09:00 PM', 'true', 4, 2, 6, 350, 25, 20, 30, 'Savory & Spicy'),
--('Chicken 65', 'Fried chicken chunks marinated with spices and deep-fried to crispy perfection.', '00:20:00', 180, '/images/chicken_65.jpg', '11:00 AM - 09:00 PM', 'true', 7, 2, 6, 400, 35, 25, 45, 'Spicy & Crispy'),
--('Chicken Tikka', 'Boneless chicken marinated with yogurt and spices, grilled to perfection.', '00:30:00', 250, '/images/chicken_tikka.jpg', '11:00 AM - 09:00 PM', 'true', 3, 2, 6, 500, 30, 18, 40, 'Savory & Smoky'),
--('Prawn Masala', 'Prawns cooked in a spicy, tangy masala with coconut and tamarind.', '00:35:00', 320, '/images/prawn_masala.jpg', '11:00 AM - 09:00 PM', 'true', 16, 2, 6, 550, 35, 20, 45, 'Spicy & Rich'),
--('Vegetable Kurma', 'Mixed vegetables cooked in a rich, creamy, and mildly spiced gravy.', '00:30:00', 220, '/images/veg_kurma.jpg', '11:00 AM - 09:00 PM', 'true', 2, 1, 6, 450, 10, 25, 60, 'Mild & Creamy'),
--('Naan with Butter Chicken', 'Soft naan served with a rich, creamy butter chicken curry.', '00:30:00', 280, '/images/naan_butter_chicken.jpg', '11:00 AM - 09:00 PM', 'true', 2, 2, 6, 650, 40, 35, 50, 'Creamy & Hearty'),
--('Pesarattu', 'Green gram pancakes served with coconut chutney and sambar.', '00:20:00', 180, '/images/pesarattu.jpg', '11:00 AM - 09:00 PM', 'true', 1, 1, 6, 350, 12, 8, 60, 'Mild & Nutty'),
--('Mutton Kebab', 'Tender mutton marinated in spices and grilled on skewers.', '00:25:00', 290, '/images/mutton_kebab.jpg', '11:00 AM - 09:00 PM', 'true', 11, 2, 6, 450, 40, 30, 40, 'Smoky & Savory'),
--('Hyderabadi Chicken Biryani', 'Flavorful biryani made with tender chicken and fragrant basmati rice.', '00:45:00', 350, '/images/hyderabadi_chicken_biryani.jpg', '11:00 AM - 09:00 PM', 'true', 4, 2, 6, 700, 45, 35, 75, 'Spicy & Rich'),
--('Chettinad Fish Curry', 'Fish cooked in a spicy, tangy Chettinad-style curry.', '00:35:00', 290, '/images/chettinad_fish_curry.jpg', '11:00 AM - 09:00 PM', 'true', 16, 2, 6, 400, 30, 15, 40, 'Spicy & Tangy'),
--('Mushroom Kurma', 'Mushrooms cooked in a creamy, mildly spiced gravy.', '00:30:00', 250, '/images/mushroom_kurma.jpg', '11:00 AM - 09:00 PM', 'true', 2, 1, 6, 450, 10, 20, 55, 'Creamy & Savory');

-- Restaurant 7: ibaco

--INSERT INTO MenuItems (
--  name, description, cookingTime, price, imagePath, availabilityTime, isAvailable,
--  cuisineId, mealTypeId, restaurantId, calories, proteins, fats, carbohydrates, tasteInfo
--) VALUES
--('Classic Chocolate Cake', 'Rich and moist chocolate cake topped with creamy chocolate frosting.', '00:30:00', 200, '/images/classic_chocolate_cake.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 450, 6, 20, 60, 'Sweet & Decadent'),
--('Vanilla Cupcakes', 'Soft and fluffy vanilla cupcakes with a light buttercream frosting.', '00:20:00', 100, '/images/vanilla_cupcakes.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 250, 4, 10, 35, 'Light & Sweet'),
--('Blueberry Cheesecake', 'Creamy cheesecake with a blueberry compote on top, served chilled.', '00:45:00', 250, '/images/blueberry_cheesecake.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 550, 8, 28, 50, 'Creamy & Fruity'),
--('Chocolate Mousse', 'Smooth and creamy chocolate mousse with a rich chocolate flavor.', '00:35:00', 180, '/images/chocolate_mousse.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 400, 5, 22, 40, 'Rich & Chocolatey'),
--('Strawberry Shortcake', 'Delicate sponge cake layered with fresh strawberries and whipped cream.', '00:25:00', 220, '/images/strawberry_shortcake.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 350, 6, 12, 45, 'Fruity & Light'),
--('Tiramisu', 'Traditional Italian dessert made with coffee-soaked ladyfingers and mascarpone cheese.', '00:30:00', 240, '/images/tiramisu.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 450, 8, 18, 50, 'Coffee-flavored & Creamy'),
--('Carrot Cake', 'Moist and spiced carrot cake with cream cheese frosting.', '00:30:00', 220, '/images/carrot_cake.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 400, 6, 15, 50, 'Sweet & Spiced'),
--('Lemon Drizzle Cake', 'Light and fluffy lemon cake with a tangy lemon drizzle topping.', '00:25:00', 180, '/images/lemon_drizzle_cake.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 300, 5, 10, 45, 'Tangy & Sweet'),
--('Peach Cobbler', 'Warm baked peaches topped with a buttery, biscuit-like crust.', '00:40:00', 220, '/images/peach_cobbler.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 350, 4, 14, 45, 'Sweet & Fruity'),
--('Apple Pie', 'Classic apple pie made with fresh apples and a buttery, flaky crust.', '00:45:00', 250, '/images/apple_pie.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 400, 5, 18, 55, 'Sweet & Buttery'),
--('Chocolate Chip Cookies', 'Soft and chewy cookies with chunks of rich chocolate chips.', '00:20:00', 120, '/images/chocolate_chip_cookies.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 300, 4, 12, 40, 'Sweet & Chewy'),
--('Raspberry Sorbet', 'A refreshing raspberry sorbet with a tangy, fruity flavor.', '00:10:00', 150, '/images/raspberry_sorbet.jpg', '11:00 AM - 09:00 PM', 'true', 15, 1, 7, 200, 1, 2, 45, 'Fruity & Refreshing'),
--('Pineapple Upside-Down Cake', 'Pineapple slices and caramelized sugar on top of a moist cake.', '00:35:00', 230, '/images/pineapple_upside_down_cake.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 400, 5, 15, 55, 'Sweet & Tangy'),
--('Mango Cheesecake', 'Creamy cheesecake made with fresh mango puree for a tropical twist.', '00:45:00', 270, '/images/mango_cheesecake.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 500, 6, 25, 55, 'Creamy & Fruity'),
--('Chocolate Lava Cake', 'Warm chocolate cake with a molten, gooey chocolate center.', '00:20:00', 230, '/images/chocolate_lava_cake.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 500, 6, 24, 50, 'Rich & Decadent'),
--('Coconut Macaroons', 'Chewy coconut macaroons baked to a golden crisp on the outside.', '00:20:00', 150, '/images/coconut_macaroons.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 300, 5, 15, 40, 'Sweet & Chewy'),
--('Chocolate Eclairs', 'Flaky pastry filled with vanilla cream and topped with a layer of chocolate.', '00:30:00', 210, '/images/chocolate_eclairs.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 350, 6, 18, 45, 'Sweet & Creamy'),
--('Banoffee Pie', 'Banana, caramel, and whipped cream layered over a buttery biscuit crust.', '00:40:00', 260, '/images/banoffee_pie.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 500, 8, 24, 60, 'Sweet & Creamy'),
--('Chocolate Fudge Brownie', 'Dense and rich chocolate brownie topped with chocolate fudge.', '00:25:00', 180, '/images/chocolate_fudge_brownie.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 450, 6, 22, 55, 'Rich & Decadent'),
--('Pistachio Baklava', 'Layers of filo pastry filled with crushed pistachios and soaked in honey syrup.', '00:30:00', 210, '/images/pistachio_baklava.jpg', '11:00 AM - 09:00 PM', 'true', 14, 1, 7, 400, 6, 20, 50, 'Sweet & Nutty');


select * from MenuItems 

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.h5URbwg5lzhoQcgjUxRT9QHaHa?cb=iwp1&rs=1&pid=ImgDetMain'
where Id = 1;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/veggie-spring-rolls-with-white-background-high-qual_889056-17629.jpg'
where Id = 2;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/chicken-doner-lavash-with-fries-with-isolated-white-background_741910-49590.jpg'
where Id = 3;

UPDATE MenuItems
SET ImagePath = 'https://lazeeznc.com/wp-content/uploads/2023/11/paneer-tikka.png'
where Id = 4;

UPDATE MenuItems
SET ImagePath = 'https://everythingorganik.in/wp-content/uploads/2020/06/veg-momo.jpg'
where Id = 5;

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.472b5ab51ITETjLvcDzlXgHaHa?cb=iwp1&rs=1&pid=ImgDetMain'
where Id = 6;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/heart-shaped-pizza-valentines-day-slate-white-background_145373-639.jpg'
where Id = 7;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/chicken-biryani-plate-isolated-white-background-delicious-spicy-biryani-isolated_667286-5784.jpg?w=2000'
where Id = 8;

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.-2dRS7VnY5_QClMIj0rV5QHaHa?cb=iwp1&rs=1&pid=ImgDetMain'
where Id = 9;

UPDATE MenuItems
SET ImagePath = 'https://static.vecteezy.com/system/resources/previews/027/735/652/non_2x/chicken-kebab-skewers-on-a-plate-on-a-transparent-background-free-png.png'
where Id = 10;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/veg-sandwich-plate-with-isolated-white-background_759707-3570.jpg'
where Id = 11;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/chicken-caesar-wrap-white-plate-white-background_864588-11088.jpg?w=2000'
where Id = 12;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/noodles-with-white-background-high-quality-ultra-hd_889056-8037.jpg?w=2000'
where Id = 13;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/chicken-burger-studio-light-isolated-white-background_300636-4817.jpg'
where Id = 14;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/samosa-near-ketchup-white-background-front-view_862994-321323.jpg'
where Id = 15;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/delicious-indian-paneer-tikka-masala-bowl-white-background-generative-ai_804788-10048.jpg'
where Id = 16;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/top-view-pasta-primavera-white-round-plate-white-background-generative-ai_918839-7116.jpg'
where Id = 17;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/pizza-with-chicken-pepperoni-it_900101-34192.jpg'
where Id = 18;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/three-tacos-plate-with-white-background_741910-1875.jpg'
where Id = 19;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/close-up-kebab-white-background-generative-ai_21085-36767.jpg'
where Id = 20;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/fry-fish-isolated-white-background_741212-2412.jpg?w=2000'
where Id = 21;


UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.0PnrpGkCvxPYonjIZ2AvRwHaFl?cb=iwp2&w=1301&h=980&rs=1&pid=ImgDetMain'
where Id = 22;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/chicken-shawarma-doner-kebab-burrito-filling-isolated-white-background_1095373-6724.jpg?w=2000'
where Id = 23;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/delicious-paneer-tikka-popular-appetizer-white-background_878783-22931.jpg'
where Id = 24;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/chow-mein-noodles-crunchy-chinese-noodles-white-background_300636-854.jpg?w=740'
where Id = 25;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/yummy-chicken-seekh-kebab-isolated-white-background_787273-55682.jpg'
where Id = 26;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/delicious-indian-paneer-tikka-masala-bowl-white-background-generative-ai_804788-10037.jpg?w=2000'
where Id = 27;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/delicious-indian-vegetable-biryani-bowl-white-background-generative-ai_804788-10024.jpg?w=2000'
where Id = 28;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/tacos-with-meat-vegetables-isolated-white-background_1339-63867.jpg?w=2000'
where Id = 29;

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.mQVhD7VHQvjvvyPK4S9pvwHaHa?cb=iwp2&w=2000&h=2000&rs=1&pid=ImgDetMain'
where Id = 30;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/crunchy-plantain-pakoras-white-background-best-indian-pakora-image-photography_1020697-180177.jpg'
where Id = 31;

UPDATE MenuItems
SET ImagePath = 'https://media.istockphoto.com/id/1176815258/photo/pasta-with-arrabiata-sauce.jpg?s=612x612&w=0&k=20&c=VQLh3Ns_1X4ZXp3Ihio6RbjgLdt6lzkkWAhGYPHfnsE='
where Id = 32;

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.YsEsxAN0k-7tnDOx7-i1VQHaHa?cb=iwp2&w=626&h=626&rs=1&pid=ImgDetMain'
where Id = 33;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/egg-fried-rice-white-plate-white-background_864588-11982.jpg'
where Id = 34;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/mushroom-risotto-isolated-white-background_881868-1540.jpg'
where Id = 35;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/mexican-red-enchiladas-plate-side-view-angle-isolated-white-background-hyper-realistic_921410-25051.jpg'
where Id = 36;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/veggie-burger-delight-white-background_729149-59332.jpg'
where Id = 37;

UPDATE MenuItems
SET ImagePath = 'https://media.istockphoto.com/id/1252605665/photo/chilli-garlic-hakka-noodles-in-black-bowl-isolated-on-white-background-indo-chinese.jpg?s=1024x1024&w=is&k=20&c=lWicrvSZX7I9G63PaaDp7Zm_5kHCav0DLl-dFvG_6jI='
where Id = 38;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/chicken-caesar-wrap-white-plate-white-background_864588-11088.jpg?w=2000'
where Id = 39;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/vegetable-soup-isolated-white-background_1368-535538.jpg'
where Id = 40;

UPDATE MenuItems
SET ImagePath = 'https://thumbs.dreamstime.com/b/indian-food-specialties-dish-kadai-shahi-paneer-lababda-105552508.jpg'
where Id = 41;

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.TUOZr9j_BQusB14QnNWKSAHaHa?cb=iwp2&w=626&h=626&rs=1&pid=ImgDetMain'
where Id = 42;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/zinger-burger-white-background_1067450-81.jpg?w=996'
where Id = 43;

UPDATE MenuItems
SET ImagePath = 'https://simply.delivery/wp-content/uploads/2020/09/KFC_variety_bucket_newfmi.jpg'
where Id = 44;

UPDATE MenuItems
SET ImagePath = 'https://kfcmenu.info/wp-content/uploads/2022/12/popcorn-1080px.png'
where Id = 45;

UPDATE MenuItems
SET ImagePath = 'https://i.gojekapi.com/darkroom/gofood-indonesia/v2/images/uploads/7f21a7e1-f4b3-4382-a810-8f54893457fa_10084.jpg?auto=format'
where Id = 46;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/fried-chicken-burger-isolated-transparent-white-background_999766-1041.jpg'
where Id = 47;

UPDATE MenuItems
SET ImagePath = 'https://images.ctfassets.net/crbk84xktnsl/4zgRg2g2ZRBey10D3qfjyZ/e9f079f486f401b884ad570be0a48af8/Zinger_Burger.png'
where Id = 48;

UPDATE MenuItems
SET ImagePath = 'https://static.kfc.co.nz/images/items/lg/popcorn-chicken-snack-box.jpg?v=g217V4'
where Id = 49;

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.Qph0dlMOJGoT_DkyxJwIbAAAAA?cb=iwp2&w=440&h=470&rs=1&pid=ImgDetMain'
where Id = 50;

UPDATE MenuItems
SET ImagePath = 'https://kfcmenu.info/wp-content/uploads/2021/02/kartofel-fri-standartnyy.png'
where Id = 51;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/fried-chicken-legs-with-french-fries-ketchup-isolated-white-background_768733-4171.jpg'
where Id = 52;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/veg-caesar-salad-isolated-white-background_759707-3626.jpg?w=2000'
where Id = 53;

UPDATE MenuItems
SET ImagePath = 'https://images.ctfassets.net/9tka4b3550oc/5YN3gUj1vZf8wynnH6aTnZ/891d6eb8eea0f0e34ba4c43c74ae85a0/9187_FmsNggtBowl_Web_RGB.png'
where Id = 54;

UPDATE MenuItems
SET ImagePath = 'https://kfc.dk/wp-content/uploads/2024/02/Hot-Wings-Bucket-for-2.png'
where Id = 55;

UPDATE MenuItems
SET ImagePath = 'https://foodmv.s3.amazonaws.com/apzYjl1W/c/11950-thumb_md.jpg'
where Id = 56;

UPDATE MenuItems
SET ImagePath = 'https://www.thedailymeal.com/img/gallery/kfcs-new-bbq-fried-chicken-sandwich-is-only-here-for-a-limited-time/intro-1687879815.jpg'
where Id = 57;

UPDATE MenuItems
SET ImagePath = 'https://qatarxplorer.com/wp-content/uploads/2023/08/653-Combo-768x768.png'
where Id = 58;

UPDATE MenuItems
SET ImagePath = 'https://www.kindpng.com/picc/m/39-394285_pineapple-fritters-hd-png-download.png'
where Id = 59;

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.1YdTDMnDc5RojyxEnIcmKwHaHa?cb=iwp2&w=626&h=626&rs=1&pid=ImgDetMain'
where Id = 60;

UPDATE MenuItems
SET ImagePath = 'https://troskit.com/wp-content/uploads/2023/06/Colonel_Rice_Bow-kfc.jpg'
where Id = 61;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/tandoori-chicken-with-white-background-high-quality_889056-34819.jpg'
where Id = 62;

UPDATE MenuItems
SET ImagePath = 'https://bk-ca-prd.s3.amazonaws.com/sites/burgerking.ca/files/03299-89_BK_Web_Triple%2520Whopper_500x540_CR.png'
where Id = 63;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/delicious-fried-chicken-with-french-fries-isolated-white-background_906385-31342.jpg'
where Id = 64;

UPDATE MenuItems
SET ImagePath = 'https://image.entabe.jp/upload/20220303/images/cheeseandcheese_2.jpg'
where Id = 65;

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.qcYAgdqMigymxhko_pfMIwHaF7?cb=iwp2&rs=1&pid=ImgDetMain'
where Id = 66;

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.iEWq5Spgu0LNF0T2ayFpmQHaH_?cb=iwp2&rs=1&pid=ImgDetMain'
where Id = 67;

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.p3ZFYHS2O9x2Up3MXLC2TAHaH_?cb=iwp2&rs=1&pid=ImgDetMain'
where Id = 68;

UPDATE MenuItems
SET ImagePath = 'https://www.seekpng.com/png/detail/52-522297_onion-rings-burger-king-onion-rings.png'
where Id = 69;

UPDATE MenuItems
SET ImagePath = 'https://media-cdn.grubhub.com/image/upload/d_search:browse-images:default.jpg/w_150,q_auto:low,fl_lossy,dpr_2.0,c_fill,f_auto,h_130/sjzv2x8xpxbir2umxjrn'
where Id = 70;

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/veggie-wrap-with-white-background-high-quality-ultr_889056-17642.jpg'
where Id = 71;

UPDATE MenuItems
SET ImagePath = 'https://burgerkingks.com/wp-content/uploads/2020/08/chicken-royale-ss.png'
where Id = 72;

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.KqR8mreUJEvVwEt7x7N7UgHaHa?cb=iwp2&rs=1&pid=ImgDetMain'
where Id = 73;

UPDATE MenuItems
SET ImagePath = 'https://static.vecteezy.com/system/resources/previews/032/325/506/non_2x/french-fries-with-cheese-and-bacon-isolated-on-transparent-background-file-cut-out-ai-generated-png.png'
where Id = 74;

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.6M8c6_4ojhjlZLXju_BrhAHaHa?cb=iwp2&w=500&h=500&rs=1&pid=ImgDetMain'
where Id = 75;

UPDATE MenuItems
SET ImagePath = 'https://www.eatatjacks.com/wp-content/uploads/2020/08/grilled-blt-salad.png'
where Id = 76;

UPDATE MenuItems
SET ImagePath = 'https://static.vecteezy.com/system/resources/previews/052/855/127/large_2x/poutine-in-a-white-bowl-transparent-background-free-png.png'
where Id = 77;

UPDATE MenuItems
SET ImagePath = 'https://thumbs.dreamstime.com/b/chicken-breast-grill-chicken-breast-lettuce-salad-zucchini-radish-isolated-white-chicken-breast-grill-chicken-breast-110724233.jpg'
where Id = 78;

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.1MNLvheosKTDQ3z-lGAEsAAAAA?cb=iwp2&rs=1&pid=ImgDetMain'
where Id = 79

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/classic-mushroom-swiss-burger-perfection-white-background_994921-2742.jpg'
where Id = 80

UPDATE MenuItems
SET ImagePath = 'https://burgerkingks.com/wp-content/uploads/2020/08/crispy-wrap.png'
where Id = 81

UPDATE MenuItems
SET ImagePath = 'https://bkmenu.co.uk/wp-content/uploads/2024/04/frozen-fanta-lemon-strawberry.png'
where Id = 82

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/delicious-mutton-biryani-isolated-white-background_787273-21913.jpg'
where Id = 83

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.FEfWRzCoVvC7rYGXjEtSpQHaHa?cb=iwp2&w=626&h=626&rs=1&pid=ImgDetMain'
where Id = 84

UPDATE MenuItems
SET ImagePath = 'https://im1.dineout.co.in/images/uploads/restaurant/sharpen/7/i/x/p78930-16082761375fdc58a99e871.jpg?tr=tr:n-xlarge'
where Id = 85

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/fish-curry-bowl-isolated-white-background_766625-9686.jpg'
where Id = 86

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/indian-food-with-white-background-high-quality-ultra_670382-116952.jpg?w=2000'
where Id = 87

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/indian-style-meat-dish-mutton-gosht-masala-lamb-rogan-josh-served-bowl-selective-focus_466689-53459.jpg'
where Id = 88

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/chicken-biryani-plate-isolated-white-background-delicious-spicy-biryani-isolated_667286-5800.jpg'
where Id = 89

UPDATE MenuItems
SET ImagePath = 'https://t4.ftcdn.net/jpg/03/64/90/27/360_F_364902762_TcAuz7SEhKKgzm32LzuE6NiRWhyHdxAx.jpg'
where Id = 90

UPDATE MenuItems
SET ImagePath = 'https://t4.ftcdn.net/jpg/03/64/90/27/360_F_364902762_TcAuz7SEhKKgzm32LzuE6NiRWhyHdxAx.jpg'
where Id = 91

UPDATE MenuItems
SET ImagePath = 'https://th.bing.com/th/id/OIP.iGm-FRBzfon3eoAAYvTDdQHaHa?cb=iwp2&w=800&h=800&rs=1&pid=ImgDetMain'
where Id = 92

UPDATE MenuItems
SET ImagePath = 'https://static.vecteezy.com/system/resources/previews/045/354/077/non_2x/culinary-perfection-chicken-tikka-masala-with-no-background-png.png'
where Id = 93

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/delightful-shorba-isolated-white-background_787273-70380.jpg'
where Id = 94

UPDATE MenuItems
SET ImagePath = 'https://thumbs.dreamstime.com/b/delicious-indian-butter-chicken-naan-bread-bowl-white-background-generative-ai-280540181.jpg'
where Id = 95

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/white-rice-with-curry-isolated-white-background_1082068-81255.jpg'
where Id = 96

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/fry-fish-isolated-white-background_741212-2412.jpg?w=2000'
where Id = 97

UPDATE MenuItems
SET ImagePath = 'https://thumbs.dreamstime.com/b/delicious-indian-paneer-tikka-masala-bowl-white-background-generative-ai-280540198.jpg'
where Id = 98

UPDATE MenuItems
SET ImagePath = 'https://png.pngtree.com/png-clipart/20231020/original/pngtree-egg-biryani-varieties-displayed-artfully-on-a-transparent-background-png-image_13388362.png'
where Id = 99

UPDATE MenuItems
SET ImagePath = 'https://st.depositphotos.com/1005682/2659/i/450/depositphotos_26599245-stock-photo-mutton-rogan-josh-mutton-curry.jpg'
where Id = 100

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/board-plate-tasty-malai-kofta-with-white-background_1111504-12819.jpg'
where Id = 101

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/pakistani-indian-traditional-curry_988987-4503.jpg'
where Id = 102

UPDATE MenuItems
SET ImagePath = 'https://img.freepik.com/premium-photo/indian-style-suran-sabzi-jimikand-sabji-also-known-as-elephant-foot-yam-ole-stir-fried-recipe_466689-83260.jpg'
where Id = 103





select * from MenuItems