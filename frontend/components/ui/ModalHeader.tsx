import React from 'react';
import IconButton from './IconButton';

export interface ModalHeaderProps {
  title: string;
  onClose?: () => void;
  showCloseButton?: boolean;
  children?: React.ReactNode;
  className?: string;
}

const ModalHeader: React.FC<ModalHeaderProps> = ({
  title,
  onClose,
  showCloseButton = true,
  children,
  className = '',
}) => {
  const CloseIcon = (
    <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
    </svg>
  );

  return (
    <div className={`flex items-center justify-between p-6 border-b border-gray-200 ${className}`}>
      <div className="flex-1">
        <h2 className="text-heading-3 text-gray-900">{title}</h2>
        {children}
      </div>
      {showCloseButton && onClose && (
        <IconButton
          variant="ghost"
          size="sm"
          icon={CloseIcon}
          onClick={onClose}
          aria-label="Close modal"
        />
      )}
    </div>
  );
};

export default ModalHeader;
