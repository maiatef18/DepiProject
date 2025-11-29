# UI/UX Design

**Project Name:** Mos3ef

The visual interface for Mos3ef has been designed to prioritize accessibility and ease of use for patients seeking urgent medical services.

## 4.1 Design Source
Detailed high-fidelity mockups and interaction flows are available in the official Figma file:
*   **Mos3ef Figma Design**

## 4.2 UI Guidelines
*   **Primary Color:** Derived from Figma design (Medical Blue tones).
*   **Typography:** Clean sans-serif fonts for readability across devices.
*   **Iconography:** Medical-specific icons used for service categories (e.g., ICU, Lab) to ensure quick recognition.

## 4.3 Key Application Screens (Mockups)

Based on the Figma design and API structure, the following key screens are defined:

### A. Landing Page / Search
*   **Hero Section:** Search bar taking center stage.
    *   *Inputs:* "Search Service/Hospital", "Select Category" (Dropdown), "Location".
*   **Results Grid:** Services displayed as cards containing:
    *   Hospital Image & Name.
    *   Service Name & Price.
    *   Star Rating (aggregated).
    *   "Compare" Checkbox.

### B. Service Comparison Modal
*   **Triggered by:** Selecting two services.
*   **Layout:** Split view (Left vs Right).
*   **Data Points:** Price Difference (Highlighted Green for cheaper), Rating Difference, Distance from User.
*   **Recommendation Engine:** A highlighted text box suggesting the better option based on the algorithm in ServiceManager.

### C. Patient Profile
*   **Left Sidebar:** Navigation (My Profile, Saved Services, Reviews).
*   **Main Content:** Form to update Name, Phone, and upload Profile Picture.
*   **Saved Services:** List of bookmarked services for quick access.

### D. Hospital Dashboard
*   **Stats Cards (Top):**
    1.  Total Services Listed.
    2.  Total Reviews Received.
    3.  Average Rating.
*   **Service Management Table:** List of services with "Edit" and "Delete" actions.
*   **Add Service Modal:** Form with fields for Name, Price, Category, and Working Hours.

## 4.4 User Experience (UX) Considerations
1.  **Feedback:** Success messages (Toast notifications) upon Registering, Saving a Service, or Posting a Review.
2.  **Error Handling:** Clear validation messages for Forms (e.g., "Password must be 6 chars").
3.  **Loading States:** Skeleton loaders while fetching data from the API.
