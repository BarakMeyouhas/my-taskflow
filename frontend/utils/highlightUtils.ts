/**
 * Highlights search terms in text by wrapping matches in <mark> tags
 * @param text - The text to search in
 * @param searchTerm - The term to highlight
 * @param caseSensitive - Whether the search should be case sensitive
 * @returns HTML string with highlighted matches
 */
export const highlightSearchTerm = (
  text: string, 
  searchTerm: string, 
  caseSensitive: boolean = false
): string => {
  if (!searchTerm.trim() || !text) {
    return text;
  }

  const flags = caseSensitive ? 'g' : 'gi';
  const regex = new RegExp(escapeRegExp(searchTerm), flags);
  
  return text.replace(regex, '<mark class="bg-yellow-200 dark:bg-yellow-800 px-1 rounded">$&</mark>');
};

/**
 * Escapes special regex characters in a string
 * @param string - The string to escape
 * @returns Escaped string safe for use in RegExp
 */
const escapeRegExp = (string: string): string => {
  return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
};

/**
 * Highlights search terms in multiple fields of a task
 * @param task - The task object
 * @param searchTerm - The search term to highlight
 * @returns Task object with highlighted text fields
 */
export const highlightTaskFields = (task: any, searchTerm: string) => {
  if (!searchTerm.trim()) {
    return task;
  }

  return {
    ...task,
    title: highlightSearchTerm(task.title, searchTerm),
    description: task.description ? highlightSearchTerm(task.description, searchTerm) : task.description,
    // Highlight tag names as well
    tags: task.tags?.map((tag: any) => ({
      ...tag,
      name: highlightSearchTerm(tag.name, searchTerm),
      description: tag.description ? highlightSearchTerm(tag.description, searchTerm) : tag.description
    })) || task.tags
  };
};

/**
 * Truncates text and adds ellipsis if it's too long
 * @param text - The text to truncate
 * @param maxLength - Maximum length before truncation
 * @returns Truncated text with ellipsis if needed
 */
export const truncateText = (text: string, maxLength: number = 100): string => {
  if (!text || text.length <= maxLength) {
    return text;
  }
  
  return text.substring(0, maxLength) + '...';
};
