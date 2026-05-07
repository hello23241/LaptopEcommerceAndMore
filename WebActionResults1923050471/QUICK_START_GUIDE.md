# E-Commerce System - Quick Start Guide

## 🚀 Getting Started

### Default Test Accounts
```
Admin Account:
  Username: admin
  Password: admin123

User Accounts:
  Username: user1 / Password: user123
  Username: user2 / Password: user456
```

### Sample Products
1. **Laptop** - $1,200.00 (Electronics)
2. **C# Programming** - $35.00 (Books)
3. **T-Shirt** - $25.00 (Clothing)

---

## 📋 Feature Overview

### 1. User Management
| Feature | Path | Description |
|---------|------|-------------|
| Register | `/Account/Register` | Create new account |
| Login | `/Account/Login` | Sign in to account |
| Profile | `/Account/Profile` | View user profile |
| Edit Profile | `/Account/Edit/{id}` | Update personal info |
| Logout | `/Account/Logout` | Sign out of account |
| View All Accounts | `/Account/Index` | Admin: View all users |

### 2. Shopping Cart
| Feature | Path | Description |
|---------|------|-------------|
| View Cart | `/Cart/Index` | View shopping cart items |
| Add to Cart | `/Cart/AddToCart` | Add product to cart |
| Update Quantity | `/Cart/UpdateQuantity` | Change item quantity |
| Remove Item | `/Cart/RemoveFromCart` | Delete item from cart |
| Checkout | `/Cart/Checkout` | Process order |
| Clear Cart | `/Cart/ClearCart` | Empty entire cart |

### 3. Product Management
| Feature | Path | Description |
|---------|------|-------------|
| Product List | `/Product/Index` | View all products |
| Product Details | `/Product/Details/{id}` | View product info |
| Create Product | `/Product/Create` | Add new product |
| Edit Product | `/Product/Edit/{id}` | Update product |
| Delete Product | `/Product/Delete/{id}` | Remove product |

### 4. Category Management
| Feature | Path | Description |
|---------|------|-------------|
| Category List | `/Category/Index` | View all categories |
| Create Category | `/Category/Create` | Add new category |
| Edit Category | `/Category/Edit/{id}` | Update category |
| Delete Category | `/Category/Delete/{id}` | Remove category |

### 5. Supplier Management
| Feature | Path | Description |
|---------|------|-------------|
| Supplier List | `/Supplier/Index` | View all suppliers |
| Create Supplier | `/Supplier/Create` | Add new supplier |
| Edit Supplier | `/Supplier/Edit/{id}` | Update supplier |
| Delete Supplier | `/Supplier/Delete/{id}` | Remove supplier |

---

## 🎯 User Workflows

### Workflow 1: Customer Shopping
```
1. Start → Homepage
2. Click "Login" in navigation
3. Enter credentials (e.g., user1 / user123)
4. Click "Products" to browse
5. Click "Add to Cart" on desired products
6. Click "Cart" in top-right to view cart
7. Adjust quantities if needed
8. Click "Proceed to Checkout"
9. Order placed - Cart clears
```

### Workflow 2: Admin Management
```
1. Start → Homepage
2. Click "Login" → Enter admin credentials (admin / admin123)
3. Use navigation bar to manage:
   - Products → Create/Edit/Delete
   - Categories → Organize products
   - Suppliers → Manage suppliers
   - Accounts → View all users
```

### Workflow 3: New User Registration
```
1. Start → Homepage
2. Click "Register" in navigation
3. Fill in registration form:
   - Username (required)
   - Full Name (required)
   - Email (required)
   - Phone (optional)
   - Birthday (optional)
   - Password (required)
4. Submit form
5. Redirected to Login page
6. Enter new credentials to login
```

---

## 🔧 Technical Details

### Session Data
```csharp
// User Session Variables:
Context.Session.GetInt32("UserId")          // Current user ID
Context.Session.GetString("UserName")       // Current username
Context.Session.GetString("FullName")       // Current user's full name

// Cart Session:
ShoppingCart_{UserId}                       // JSON stored in session
```

### Key Services
```
IDataService          - Product/Category/Supplier/Account operations
ICartService          - Shopping cart management with session storage
```

### Controllers
- `ProductController` - Product CRUD operations
- `CategoryController` - Category management
- `SupplierController` - Supplier management
- `AccountController` - User authentication and profiles
- `CartController` - Shopping cart operations

---

## 💡 Tips & Tricks

### Adding to Cart
- Use "Add to Cart" button from product list (logged-in users only)
- Guests must login first
- Quantity can be updated in cart view

### Managing Products
- Products can be created with category and supplier selection
- Edit product to change category or supplier
- Delete removes product completely

### Cart Management
- Each user has independent cart session
- Cart data stored in memory (clears on app restart)
- Checkout clears cart automatically

### User Profiles
- Edit profile updates user information
- Users can only edit their own profiles
- Admin can view and manage all accounts

---

## 🎨 UI Components

### Navigation Bar
- **Logo** - Returns to homepage
- **Main Links** - Products, Categories, Suppliers
- **User Section** - Login/Register or user dropdown menu
- **Cart Link** - Quick access to shopping cart (logged-in users)

### Product Table
- Product ID, Name, Price, Quantity, Category
- View, Edit, Delete, Add to Cart buttons
- Different styles for action buttons

### Cart Display
- Product name, price, quantity, subtotal
- Update quantity and remove options
- Cart summary with totals
- Checkout and Clear Cart buttons

### User Forms
- Bootstrap-styled form controls
- Validation message display
- Required field indicators
- Submit and Cancel buttons

---

## ⚠️ Important Notes

1. **In-Memory Storage** - Data resets when app restarts
2. **Session Timeout** - 30 minutes of inactivity
3. **Password Storage** - Currently stored as plain text (implement hashing in production)
4. **Authorization** - Cart operations require login
5. **Cart Capacity** - No stock validation (can exceed available quantity)

---

## 📞 Support

For issues or questions:
1. Check the IMPLEMENTATION_GUIDE.md
2. Review the DATABASE_SCHEMA.sql for structure
3. Check controller logic for business rules
4. Verify session is initialized before cart operations

---

## 🔐 Security Reminders

- ✅ Session-based authentication
- ✅ HTTPS redirect enabled
- ✅ CSRF protection built-in
- ⚠️ TODO: Implement password hashing
- ⚠️ TODO: Add role-based access control
- ⚠️ TODO: Implement SQL injection prevention (if using DB)
