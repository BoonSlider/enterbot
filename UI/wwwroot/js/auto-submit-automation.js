// Fuck this garbage - triggering a blazor click handler from js seems impossible.

// document.addEventListener("DOMContentLoaded", function () {
//     // Use MutationObserver to handle dynamically added forms
//     const observer = new MutationObserver(function (mutations) {
//         // Find all automation-form inputs in the document
//         document.querySelectorAll('.automation-form input').forEach(input => {
//             // Only add the listener if we haven't already (using a custom attribute to track)
//             if (!input.hasAttribute('data-auto-submit-attached')) {
//                 input.setAttribute('data-auto-submit-attached', 'true');
//                 input.addEventListener('change', function () {
//                     // Find the parent form and submit it
//                     const form = input.closest('.automation-form');
//                     if (form) {
//                         form.submit();
//                     }
//                 });
//             }
//         });
//     });
//
//     // Start observing the entire document for changes
//     observer.observe(document.body, {
//         childList: true,
//         subtree: true
//     });
//
//     // Also handle any inputs that are already in the DOM
//     document.querySelectorAll('.automation-form input').forEach(input => {
//         input.setAttribute('data-auto-submit-attached', 'true');
//         input.addEventListener('change', function () {
//             const form = input.closest('.automation-form');
//             if (form) {
//                 triggerBlazorButtonClick('.automation-save-button');
//             }
//         });
//     });
// });
// function triggerBlazorButtonClick(selector) {
//     const button = document.querySelector(selector);
//     if (!button) {
//         console.error('Button not found:', selector);
//         return;
//     }
//
//     // Try to prevent default form submission behavior
//     const originalSubmit = HTMLFormElement.prototype.submit;
//     HTMLFormElement.prototype.submit = function() {
//         console.log('Form submission intercepted');
//         // Do nothing to prevent the actual submission
//     };
//
//     try {
//         // This approach uses all three event triggering methods
//         // 1. Try direct method invocation if it's a Blazor component
//         if (typeof button._blazorEvents !== 'undefined') {
//             const eventHandlerId = Object.keys(button._blazorEvents || {})[0];
//             if (eventHandlerId) {
//                 button._blazorEvents[eventHandlerId].forEach(callback => callback());
//                 return;
//             }
//         }
//
//         // 2. Try data attributes approach
//         const onclickValue = button.getAttribute('onclick');
//         if (onclickValue && onclickValue.includes('Blazor')) {
//             // Execute the onclick handler directly
//             new Function(onclickValue)();
//             return;
//         }
//
//         // 3. Use the most comprehensive event simulation
//         ['mousedown', 'mouseup', 'click'].forEach(eventType => {
//             const event = new MouseEvent(eventType, {
//                 view: window,
//                 bubbles: true,
//                 cancelable: true,
//                 buttons: eventType === 'mouseup' ? 0 : 1
//             });
//             button.dispatchEvent(event);
//         });
//     } finally {
//         // Restore original submit behavior
//         HTMLFormElement.prototype.submit = originalSubmit;
//     }
// }