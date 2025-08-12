import React from "react";
import Link from "next/link";

const Footer: React.FC = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="mt-16 lg:ml-64 border-t border-gray-200/80 bg-white/60 backdrop-blur supports-[backdrop-filter]:bg-white/40">
      <div className="mx-auto w-full max-w-7xl px-4 sm:px-6 lg:px-8 py-6">
        <div className="flex flex-col sm:flex-row items-center justify-between gap-3">
          <p className="text-xs sm:text-sm text-gray-500">
            © {currentYear} TaskFlow. All rights reserved.
          </p>
          <nav className="flex items-center gap-4">
            <Link href="#" className="text-xs sm:text-sm text-gray-500 hover:text-gray-700 transition-colors">Privacy</Link>
            <span className="hidden sm:inline text-gray-300">•</span>
            <Link href="#" className="text-xs sm:text-sm text-gray-500 hover:text-gray-700 transition-colors">Terms</Link>
            <span className="hidden sm:inline text-gray-300">•</span>
            <Link href="#" className="text-xs sm:text-sm text-gray-500 hover:text-gray-700 transition-colors">Contact</Link>
          </nav>
        </div>
      </div>
    </footer>
  );
};

export default Footer;