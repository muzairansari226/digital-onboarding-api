# Stored procedures

Hand-written procedures live here, one file each, every one `CREATE OR ALTER` so re-running is
safe. They are applied after EF migrations by a post-deploy step, never by the application at
startup.

**This folder is currently empty, and that is deliberate.** The split is EF Core for single-entity
CRUD, Dapper plus a stored procedure for paged, filtered or aggregated reads — and the onboarding
journey has no read of that shape. Every operation in it is a single-entity write or a lookup by
key, which EF handles well.

`ISqlConnectionFactory` is registered and ready in `CustomerPortal.Infrastructure/Data`, so the
first genuine paged read has somewhere to land without any new plumbing. When you add one:

- return two result sets from the procedure — the page, then the total count;
- page with `OFFSET`/`FETCH` under a deterministic `ORDER BY`, including `RecId` as a tiebreaker,
  or rows will repeat across pages;
- whitelist sort columns with `CASE`. Never concatenate an identifier or a value into SQL text.
