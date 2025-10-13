/**
 * FAQ JavaScript Module
 * Handles search, accordion, and interaction functionality
 */

class FAQManager {
    constructor() {
        this.searchInput = document.getElementById('faqSearch');
        this.clearButton = document.getElementById('clearSearch');
        this.faqItems = document.querySelectorAll('.faq-item');
        this.searchTimeout = null;
        
        this.init();
    }
    
    init() {
        this.bindEvents();
        this.setupAccordion();
    }
    
    bindEvents() {
        // Search functionality
        if (this.searchInput) {
            this.searchInput.addEventListener('input', (e) => {
                this.handleSearch(e.target.value);
            });
            
            this.searchInput.addEventListener('focus', () => {
                this.searchInput.parentElement.classList.add('focused');
            });
            
            this.searchInput.addEventListener('blur', () => {
                this.searchInput.parentElement.classList.remove('focused');
            });
        }
        
        // Clear search
        if (this.clearButton) {
            this.clearButton.addEventListener('click', () => {
                this.clearSearch();
            });
        }
        
        // Keyboard navigation
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.searchInput && this.searchInput.value) {
                this.clearSearch();
            }
        });
    }
    
    setupAccordion() {
        // FAQ accordion functionality
        document.querySelectorAll('.faq-question').forEach(question => {
            question.addEventListener('click', (e) => {
                this.toggleAnswer(e.currentTarget);
            });
            
            // Keyboard accessibility
            question.addEventListener('keydown', (e) => {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    this.toggleAnswer(e.currentTarget);
                }
            });
            
            // Make focusable
            question.setAttribute('tabindex', '0');
            question.setAttribute('role', 'button');
            question.setAttribute('aria-expanded', 'false');
        });
    }
    
    toggleAnswer(questionElement) {
        const answer = questionElement.nextElementSibling;
        const icon = questionElement.querySelector('.faq-icon');
        const isExpanded = answer.style.display === 'block';
        
        // Close other answers
        document.querySelectorAll('.faq-answer').forEach(otherAnswer => {
            if (otherAnswer !== answer) {
                otherAnswer.style.display = 'none';
                const otherIcon = otherAnswer.previousElementSibling.querySelector('.faq-icon');
                otherIcon.classList.remove('rotated');
                otherAnswer.previousElementSibling.setAttribute('aria-expanded', 'false');
            }
        });
        
        // Toggle current answer
        if (isExpanded) {
            answer.style.display = 'none';
            icon.classList.remove('rotated');
            questionElement.setAttribute('aria-expanded', 'false');
        } else {
            answer.style.display = 'block';
            icon.classList.add('rotated');
            questionElement.setAttribute('aria-expanded', 'true');
            
            // Smooth scroll to answer if it's not fully visible
            setTimeout(() => {
                const rect = answer.getBoundingClientRect();
                const isVisible = rect.top >= 0 && rect.bottom <= window.innerHeight;
                
                if (!isVisible) {
                    answer.scrollIntoView({ 
                        behavior: 'smooth', 
                        block: 'nearest' 
                    });
                }
            }, 100);
        }
    }
    
    handleSearch(query) {
        // Clear previous timeout
        if (this.searchTimeout) {
            clearTimeout(this.searchTimeout);
        }
        
        // Add loading state
        this.addLoadingState();
        
        // Debounce search
        this.searchTimeout = setTimeout(() => {
            this.performSearch(query.trim().toLowerCase());
            this.removeLoadingState();
        }, 300);
    }
    
    performSearch(query) {
        let hasResults = false;
        
        this.faqItems.forEach(item => {
            const questionText = item.querySelector('.faq-question-text').textContent.toLowerCase();
            const answerText = item.querySelector('.faq-answer-content').textContent.toLowerCase();
            
            if (questionText.includes(query) || answerText.includes(query)) {
                item.style.display = 'block';
                this.highlightText(item.querySelector('.faq-question-text'), query);
                this.highlightText(item.querySelector('.faq-answer-content'), query);
                hasResults = true;
                
                // Auto-expand matching questions
                if (query.length > 2) {
                    const answer = item.querySelector('.faq-answer');
                    const icon = item.querySelector('.faq-icon');
                    answer.style.display = 'block';
                    icon.classList.add('rotated');
                    item.querySelector('.faq-question').setAttribute('aria-expanded', 'true');
                }
            } else {
                item.style.display = 'none';
                this.removeHighlight(item.querySelector('.faq-question-text'));
                this.removeHighlight(item.querySelector('.faq-answer-content'));
            }
        });
        
        // Show/hide clear button
        if (this.clearButton) {
            this.clearButton.style.display = query ? 'flex' : 'none';
        }
        
        // Show empty state if no results
        this.showEmptyState(!hasResults && query.length > 0);
    }
    
    clearSearch() {
        if (this.searchInput) {
            this.searchInput.value = '';
        }
        
        this.resetSearch();
        
        if (this.clearButton) {
            this.clearButton.style.display = 'none';
        }
        
        this.showEmptyState(false);
    }
    
    resetSearch() {
        this.faqItems.forEach(item => {
            item.style.display = 'block';
            this.removeHighlight(item.querySelector('.faq-question-text'));
            this.removeHighlight(item.querySelector('.faq-answer-content'));
            
            // Close all answers
            const answer = item.querySelector('.faq-answer');
            const icon = item.querySelector('.faq-icon');
            answer.style.display = 'none';
            icon.classList.remove('rotated');
            item.querySelector('.faq-question').setAttribute('aria-expanded', 'false');
        });
    }
    
    highlightText(element, query) {
        if (!element || !query) return;
        
        const text = element.textContent;
        const regex = new RegExp(`(${this.escapeRegex(query)})`, 'gi');
        const highlighted = text.replace(regex, '<mark>$1</mark>');
        element.innerHTML = highlighted;
    }
    
    removeHighlight(element) {
        if (!element) return;
        
        const html = element.innerHTML;
        const cleanHtml = html.replace(/<mark>(.*?)<\/mark>/gi, '$1');
        element.innerHTML = cleanHtml;
    }
    
    escapeRegex(string) {
        return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    }
    
    addLoadingState() {
        if (this.searchInput) {
            this.searchInput.parentElement.classList.add('searching');
        }
    }
    
    removeLoadingState() {
        if (this.searchInput) {
            this.searchInput.parentElement.classList.remove('searching');
        }
    }
    
    showEmptyState(show) {
        let emptyState = document.querySelector('.faq-empty-state');
        
        if (show && !emptyState) {
            emptyState = document.createElement('div');
            emptyState.className = 'faq-empty-state';
            emptyState.innerHTML = `
                <i class="fas fa-search"></i>
                <h4>Không tìm thấy câu hỏi</h4>
                <p>Hãy thử tìm kiếm với từ khóa khác</p>
            `;
            
            const container = document.querySelector('.faq-category');
            if (container) {
                container.appendChild(emptyState);
            }
        } else if (!show && emptyState) {
            emptyState.remove();
        }
    }
}

// Initialize FAQ when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    new FAQManager();
});

// Export for potential use in other modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = FAQManager;
} 