# CineBook Frontend Redesign Changelog

## Frontend Improvements

- Rebuilt the shared Razor layout with a premium CineBook identity, sticky navigation, accessible skip link, role-aware admin/canteen menus, and a polished footer.
- Added Poppins typography and Bootstrap Icons while keeping Bootstrap 5 as the UI foundation.
- Split styling into `theme.css`, `layout.css`, `components.css`, `pages.css`, `admin.css`, and `responsive.css`, with `site.css` reduced to baseline document styles.
- Created reusable Razor partials for movie cards and empty states to reduce repeated markup.
- Redesigned the home page with a cinematic hero, product highlights, and a clear call to action.
- Redesigned the movies catalog with modern cards, genre badges, duration metadata, status counts, empty states, and improved spacing.
- Redesigned movie details with richer poster presentation, metadata pills, and card-based showtime booking actions.
- Redesigned seat selection with premium pricing summary, styled seat map, clearer selected-seat summary, responsive behavior, and preserved existing JavaScript behavior.
- Redesigned booking confirmation with success state, structured booking details, seat badges, and action buttons.
- Redesigned My Bookings with modern data tables, status badges, and an improved empty state.
- Redesigned login and register screens with centered auth cards, icon-enhanced inputs, validation support, and accessible form structure.
- Redesigned admin movie, employee, and snack listing pages with admin toolbars, filter panels, modern tables, status badges, and action groups.
- Redesigned admin create/edit forms for movies, showtimes, employees, and snacks with consistent form panels and action styling.
- Redesigned canteen dashboard tables for order workflow and snack availability management.
- Redesigned snack ordering with delivery-seat card, modern snack table, and improved empty state.
- Redesigned privacy and error pages to match the CineBook visual system.

## Quality Checks

- Backend code, controllers, models, DTOs, services, routing, authorization, database, and business logic were not modified.
- Existing form actions, posted field names, antiforgery tokens, validation partials, and JavaScript selectors were preserved.
- `dotnet build` completed successfully with 0 warnings and 0 errors.
- Local HTTP smoke checks returned 200 for home, login, movies, and the responsive CSS asset.
- The in-app browser visual verification could not run because the browser bridge was blocked by a Windows permission boundary under `AppData`; static responsive rules and server-rendered assets were verified through build and HTTP checks.
