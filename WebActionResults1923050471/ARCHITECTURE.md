# System Architecture & Flow Diagrams

## 🏗️ Application Architecture

```
┌─────────────────────────────────────────────────────┐
│                 ASP.NET Core MVC                    │
├─────────────────────────────────────────────────────┤
│                                                     │
│  ┌────────────────────────────────────────────┐   │
│  │           Controllers (5)                  │   │
│  │  ├─ ProductController                     │   │
│  │  ├─ CategoryController                    │   │
│  │  ├─ SupplierController                    │   │
│  │  ├─ AccountController                     │   │
│  │  └─ CartController                        │   │
│  └────────────────────────────────────────────┘   │
│                        ↓                            │
│  ┌────────────────────────────────────────────┐   │
│  │         Services (2)                       │   │
│  │  ├─ IDataService                           │   │
│  │  │  └─ InMemoryDataService                 │   │
│  │  └─ ICartService                           │   │
│  │     └─ CartService                         │   │
│  └────────────────────────────────────────────┘   │
│                        ↓                            │
│  ┌────────────────────────────────────────────┐   │
│  │         Models (5)                         │   │
│  │  ├─ Product                                │   │
│  │  ├─ Category                               │   │
│  │  ├─ Supplier                               │   │
│  │  ├─ Account                                │   │
│  │  └─ Cart/CartItem                          │   │
│  └────────────────────────────────────────────┘   │
│                        ↓                            │
│  ┌────────────────────────────────────────────┐   │
│  │         Session Storage                    │   │
│  │  ├─ UserId                                 │   │
│  │  ├─ UserName                               │   │
│  │  ├─ FullName                               │   │
│  │  └─ ShoppingCart (JSON)                    │   │
│  └────────────────────────────────────────────┘   │
│                                                     │
└─────────────────────────────────────────────────────┘
```

---

## 🔄 User Authentication Flow

```
┌─────────────┐
│   Guest     │
└──────┬──────┘
       │
       ├─→ Register ──→ Account/Register
       │     │
       │     └─→ Validate & Create Account
       │
       └─→ Login ──→ Account/Login
             │
             ├─→ Enter Credentials
             │
             ├─→ Validate in DataService
             │
             ├─→ Check Account Status
             │
             ├─→ Success ──→ Create Session
             │              ├─ UserId
             │              ├─ UserName
             │              └─ FullName
             │
             └─→ Failure ──→ Show Error

┌──────────────────┐
│  Authenticated   │
│  User (Session)  │
└──────────────────┘
     │
     ├─→ Profile ──→ Account/Profile
     │
     ├─→ Shop ──→ Product/Index
     │
     ├─→ Cart ──→ Cart/Index
     │
     └─→ Logout ──→ Clear Session
```

---

## 🛒 Shopping Cart Flow

```
┌──────────────────┐
│  Logged-in User  │
└────────┬─────────┘
         │
         ├─→ View Products
         │     │
         │     ├─→ Product/Index
         │     │
         │     └─→ Show "Add to Cart" Button
         │
         ├─→ Add to Cart
         │     │
         │     ├─→ Cart/AddToCart (POST)
         │     │
         │     ├─→ Retrieve Product from DataService
         │     │
         │     ├─→ Create CartItem
         │     │
         │     ├─→ Get Cart from Session
         │     │
         │     ├─→ Add Item to Cart
         │     │
         │     ├─→ Save Cart to Session
         │     │
         │     └─→ Redirect to Cart/Index
         │
         ├─→ View Cart
         │     │
         │     ├─→ Cart/Index
         │     │
         │     ├─→ Retrieve Cart from Session
         │     │
         │     └─→ Display Items + Summary
         │
         ├─→ Update Quantity
         │     │
         │     ├─→ Cart/UpdateQuantity (POST)
         │     │
         │     ├─→ Update CartItem.Quantity
         │     │
         │     ├─→ Save Cart to Session
         │     │
         │     └─→ Redirect to Cart/Index
         │
         ├─→ Remove Item
         │     │
         │     ├─→ Cart/RemoveFromCart (POST)
         │     │
         │     ├─→ Remove from Cart.Items
         │     │
         │     ├─→ Save Cart to Session
         │     │
         │     └─→ Redirect to Cart/Index
         │
         └─→ Checkout
               │
               ├─→ Cart/Checkout (POST)
               │
               ├─→ Validate Cart
               │
               ├─→ Clear Cart from Session
               │
               ├─→ Show Success Message
               │
               └─→ Redirect to Product/Index
```

---

## 📊 Data Model Relationships

```
┌──────────────────┐
│    Account       │
├──────────────────┤
│ Id (PK)          │
│ UserName         │
│ FullName         │
│ Password         │
│ Email            │
│ Phone            │
│ Birthday         │
│ Status           │
│ Notes            │
│ CreatedDate      │
└──────────────────┘

┌──────────────────┐        ┌──────────────────┐
│    Category      │        │    Supplier      │
├──────────────────┤        ├──────────────────┤
│ Id (PK)          │        │ Id (PK)          │
│ Name             │        │ Name             │
│ Description      │        │ ContactInfo      │
└──────────────────┘        │ Address          │
        ↑                    └──────────────────┘
        │                            ↑
        │                            │
        │    ┌──────────────────┐    │
        └────┤    Product       │────┘
             ├──────────────────┤
             │ Id (PK)          │
             │ Name             │
             │ Description      │
             │ Price            │
             │ Quantity         │
             │ CategoryId (FK)  │
             │ SupplierId (FK)  │
             └──────────────────┘

┌──────────────────────┐
│  ShoppingCart        │
├──────────────────────┤
│ AccountId            │
│ Items[]              │
│  ├─ ProductId       │
│  ├─ ProductName     │
│  ├─ Price           │
│  ├─ Quantity        │
│  └─ Subtotal        │
├──────────────────────┤
│ TotalItems (calc)    │
│ TotalPrice (calc)    │
└──────────────────────┘
```

---

## 📄 Page Navigation Map

```
                    ┌─────────────┐
                    │    Home     │
                    └──────┬──────┘
                           │
         ┌─────────────────┼─────────────────┐
         │                 │                 │
    ┌────▼─────┐     ┌────▼────┐      ┌────▼─────┐
    │ Products │     │ Login  ◄─┤      │ Register │
    │ Browsing │     │ Page   │ │      │ Page     │
    └────┬─────┘     └────┬───┘ │      └────┬─────┘
         │                │     │           │
         │                └─────┼───────────┘
         │                      │
         ├─ Add to Cart ─→ ┌────▼────────────────┐
         │ (requires login)│  Authenticated User │
         │                └────┬─────────────────┘
         │                     │
         │                 ┌───┴────────┬─────┐
         │                 │            │     │
         │           ┌─────▼──┐  ┌─────▼──┐  │
         │           │ Profile│  │  Cart  │  │
         │           │ Pages  │  │ Pages  │  │
         │           └─────┬──┘  └─────┬──┘  │
         │                 │           │     │
         │                 ├─→ Checkout─┘    │
         │                 │           │     │
         │                 └─ Edit ────┘     │
         │                           │       │
         │                      ┌────▼───┐   │
         └──────────────────────►Logout  ◄───┘
                            ┌───┴──────┐
                            │  Return  │
                            │   Home   │
                            └──────────┘
```

---

## 🔐 Session Management

```
┌─────────────────────────────────────────────────────┐
│            Session Storage (Server-Side)            │
├─────────────────────────────────────────────────────┤
│                                                     │
│  On Login:                                          │
│  ├─ Session.SetInt32("UserId", account.Id)         │
│  ├─ Session.SetString("UserName", account.UserName)│
│  └─ Session.SetString("FullName", account.FullName)│
│                                                     │
│  On Add to Cart:                                    │
│  └─ Session.SetString("ShoppingCart_" + userId, ...)│
│                                                     │
│  On Logout or Timeout (30 min):                    │
│  └─ Session.Clear()                                │
│                                                     │
└─────────────────────────────────────────────────────┘
```

---

## 🎯 Controller Action Flow

```
ProductController
├── Index() ──────────→ GET /Product
├── Details(id) ──────→ GET /Product/Details/{id}
├── Create() ─────────→ GET /Product/Create
├── Create(product) ──→ POST /Product/Create
├── Edit(id) ─────────→ GET /Product/Edit/{id}
├── Edit(id, product)→ POST /Product/Edit/{id}
├── Delete(id) ───────→ GET /Product/Delete/{id}
└── Delete(id) ───────→ POST /Product/Delete/{id}

AccountController
├── Login() ──────────→ GET /Account/Login
├── Login(user, pwd) →→ POST /Account/Login
├── Register() ───────→ GET /Account/Register
├── Register(acc) ────→ POST /Account/Register
├── Profile() ────────→ GET /Account/Profile
├── Edit(id) ─────────→ GET /Account/Edit/{id}
├── Edit(id, acc) ────→ POST /Account/Edit/{id}
├── Logout() ─────────→ GET /Account/Logout
├── Index() ──────────→ GET /Account/Index
├── Details(id) ──────→ GET /Account/Details/{id}
├── Delete(id) ───────→ GET /Account/Delete/{id}
└── Delete(id) ───────→ POST /Account/Delete/{id}

CartController
├── Index() ──────────→ GET /Cart
├── AddToCart(id) ────→ POST /Cart/AddToCart
├── RemoveFromCart(id)→ POST /Cart/RemoveFromCart
├── UpdateQuantity(..)→ POST /Cart/UpdateQuantity
├── Checkout() ───────→ POST /Cart/Checkout
└── ClearCart() ──────→ POST /Cart/ClearCart
```

---

## 🔄 Request/Response Cycle

```
        Client Browser
               │
               │ HTTP Request
               ↓
    ┌──────────────────────┐
    │    Routing Engine    │
    └──────────┬───────────┘
               │
               ↓
    ┌──────────────────────┐
    │   Controller Action  │
    │  (Receives Request)  │
    └──────────┬───────────┘
               │
               ├─→ Validate Input
               │
               ├─→ Call Service Layer
               │
               ├─→ Process Business Logic
               │
               ├─→ Prepare Model
               │
               ├─→ Select View
               │
               ↓
    ┌──────────────────────┐
    │    View Engine       │
    │ (Render HTML)        │
    └──────────┬───────────┘
               │
               ↓ HTTP Response
        Client Browser
        (Display Page)
```

---

## 📈 Data Flow Summary

```
User Input (Forms)
        ↓
    Controller
        ↓
    Service Layer
        ↓
    Data Storage
    (In-Memory +
     Session)
        ↓
    Service Layer
        ↓
    View Model
        ↓
    View (Razor Page)
        ↓
    HTML Response
        ↓
    Browser Display
```

This architecture provides:
✅ Separation of concerns
✅ Testability
✅ Maintainability
✅ Scalability
✅ Reusability
