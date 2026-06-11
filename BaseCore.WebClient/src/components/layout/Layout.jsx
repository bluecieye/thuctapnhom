

import { Outlet } from 'react-router-dom';

import Header from './Header';
import Footer from './Footer';

// ════════════════════════════════════════════════════════════
// COMPONENT LAYOUT (KHUNG TRANG)
// ════════════════════════════════════════════════════════════
const Layout = () => {
  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (

    

    <div className="flex min-h-screen flex-col bg-white">
      {}
      <Header />

      {}
      <main className="flex-grow pt-20 lg:pt-24">
        {}
        <Outlet />
      </main>

      {}
      <Footer />
    </div>
  );
};

export default Layout;
