import React from 'react';

export interface ModalBodyProps {
  children: React.ReactNode;
  className?: string;
  scrollable?: boolean;
}

const ModalBody: React.FC<ModalBodyProps> = ({
  children,
  className = '',
  scrollable = true,
}) => {
  const scrollClasses = scrollable ? 'overflow-y-auto' : '';
  
  return (
    <div className={`p-6 ${scrollClasses} ${className}`}>
      {children}
    </div>
  );
};

export default ModalBody;
