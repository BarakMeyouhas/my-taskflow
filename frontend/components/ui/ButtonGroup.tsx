import React from 'react';

export interface ButtonGroupProps {
  children: React.ReactNode;
  orientation?: 'horizontal' | 'vertical';
  spacing?: 'sm' | 'md' | 'lg';
  className?: string;
}

const ButtonGroup: React.FC<ButtonGroupProps> = ({
  children,
  orientation = 'horizontal',
  spacing = 'md',
  className = '',
}) => {
  const orientationClasses = {
    horizontal: 'flex-row',
    vertical: 'flex-col',
  };
  
  const spacingClasses = {
    sm: 'gap-1',
    md: 'gap-2',
    lg: 'gap-3',
  };
  
  const classes = [
    'flex',
    orientationClasses[orientation],
    spacingClasses[spacing],
    className,
  ].filter(Boolean).join(' ');
  
  return (
    <div className={classes}>
      {children}
    </div>
  );
};

export default ButtonGroup;
