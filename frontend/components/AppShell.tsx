"use client";

import React, { useState } from "react";
import Header from "./Header";
import Sidebar from "./Sidebar";

interface AppShellProps {
  children: React.ReactNode;
}

const AppShell: React.FC<AppShellProps> = ({ children }) => {
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Fixed header */}
      <div className="fixed top-0 inset-x-0 z-50">
        <Header onMenuClick={() => setIsSidebarOpen((v) => !v)} />
      </div>

      {/* Sidebar - fixed on desktop, slide-in on mobile */}
      <div
        className={
          "fixed top-16 left-0 z-40 h-[calc(100vh-4rem)] w-64 transform border-r border-gray-200 bg-white transition-transform duration-200 ease-in-out " +
          (isSidebarOpen ? "translate-x-0" : "-translate-x-full") +
          " lg:translate-x-0"
        }
      >
        <Sidebar />
      </div>

      {/* Mobile overlay */}
      {isSidebarOpen && (
        <div
          className="fixed inset-0 z-30 bg-black/40 lg:hidden"
          onClick={() => setIsSidebarOpen(false)}
        />
      )}

      {/* Main content area, offset for header and sidebar */}
      <main className="pt-16 lg:ml-64 min-h-screen">
        {children}
      </main>
    </div>
  );
};

export default AppShell;


