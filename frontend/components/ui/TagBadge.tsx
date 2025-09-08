import React from 'react';
import { Tag } from '../../types/tag';

export interface TagBadgeProps {
  tag: Tag;
  size?: 'sm' | 'md' | 'lg';
  removable?: boolean;
  onRemove?: (tagId: number) => void;
  className?: string;
}

const TagBadge: React.FC<TagBadgeProps> = ({
  tag,
  size = 'md',
  removable = false,
  onRemove,
  className = ''
}) => {
  const sizeClasses = {
    sm: 'px-2 py-1 text-xs',
    md: 'px-2.5 py-1 text-sm',
    lg: 'px-3 py-1.5 text-base'
  };

  const baseClasses = 'inline-flex items-center gap-1.5 rounded-full font-medium transition-colors';
  
  // Use tag color if available, otherwise use default colors
  const getTagStyles = () => {
    if (tag.color) {
      return {
        backgroundColor: tag.color,
        color: getContrastColor(tag.color)
      };
    }
    
    // Default color scheme based on tag name hash
    const hash = tag.name.split('').reduce((a, b) => {
      a = ((a << 5) - a) + b.charCodeAt(0);
      return a & a;
    }, 0);
    
    const colors = [
      'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
      'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
      'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
      'bg-pink-100 text-pink-800 dark:bg-pink-900 dark:text-pink-200',
      'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
      'bg-indigo-100 text-indigo-800 dark:bg-indigo-900 dark:text-indigo-200',
      'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
      'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
    ];
    
    return colors[Math.abs(hash) % colors.length];
  };

  const handleRemove = (e: React.MouseEvent) => {
    e.stopPropagation();
    onRemove?.(tag.id);
  };

  const tagStyles = tag.color ? getTagStyles() : getTagStyles();

  return (
    <span
      className={`${baseClasses} ${sizeClasses[size]} ${tagStyles} ${className}`}
      style={tag.color ? { backgroundColor: tag.color, color: getContrastColor(tag.color) } : undefined}
      title={tag.description || tag.name}
    >
      {tag.name}
      {removable && (
        <button
          onClick={handleRemove}
          className="ml-1 hover:opacity-70 transition-opacity"
          aria-label={`Remove ${tag.name} tag`}
        >
          <svg className="w-3 h-3" fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clipRule="evenodd" />
          </svg>
        </button>
      )}
    </span>
  );
};

// Helper function to determine if text should be light or dark based on background color
function getContrastColor(hexColor: string): string {
  // Remove # if present
  const hex = hexColor.replace('#', '');
  
  // Convert to RGB
  const r = parseInt(hex.substr(0, 2), 16);
  const g = parseInt(hex.substr(2, 2), 16);
  const b = parseInt(hex.substr(4, 2), 16);
  
  // Calculate luminance
  const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
  
  // Return white for dark backgrounds, black for light backgrounds
  return luminance > 0.5 ? '#000000' : '#ffffff';
}

export default TagBadge;
