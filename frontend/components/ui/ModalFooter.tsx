import React from 'react';

export interface ModalFooterProps {
  children: React.ReactNode;
  className?: string;
  align?: 'left' | 'center' | 'right' | 'between';
}

const ModalFooter: React.FC<ModalFooterProps> = ({
  children,
  className = '',
  align = 'right',
}) => {
  const alignClasses = {
    left: 'justify-start',
    center: 'justify-center',
    right: 'justify-end',
    between: 'justify-between',
  };

  return (
    <div className={`flex items-center gap-3 p-6 border-t border-gray-200 ${alignClasses[align]} ${className}`}>
      {children}
    </div>
  );
};

export default ModalFooter;
