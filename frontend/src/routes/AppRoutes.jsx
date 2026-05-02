import { Route, Routes } from 'react-router-dom';
import Dashboard from '../pages/admin/Dashboard';
import ManageCategories from '../pages/admin/ManageCategories';
import ManageProducts from '../pages/admin/ManageProducts';
import Login from '../pages/auth/Login';
import Register from '../pages/auth/Register';
import Cart from '../pages/customer/Cart';
import Checkout from '../pages/customer/Checkout';
import Home from '../pages/customer/Home';
import MyOrders from '../pages/customer/MyOrders';
import ProductDetails from '../pages/customer/ProductDetails';
import ProductList from '../pages/customer/ProductList';

function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route path="/products" element={<ProductList />} />
      <Route path="/products/:id" element={<ProductDetails />} />
      <Route path="/cart" element={<Cart />} />
      <Route path="/checkout" element={<Checkout />} />
      <Route path="/orders" element={<MyOrders />} />
      <Route path="/admin/dashboard" element={<Dashboard />} />
      <Route path="/admin/categories" element={<ManageCategories />} />
      <Route path="/admin/products" element={<ManageProducts />} />
    </Routes>
  );
}

export default AppRoutes;
