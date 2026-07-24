-- PostgreSQL 15+
-- Core schema for multi-tenant notary management platform

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

CREATE SCHEMA IF NOT EXISTS core;

-- =========================================================
-- ENUM TYPES
-- =========================================================

DO $$
BEGIN
    IF to_regtype('core.tenant_status') IS NULL THEN
        CREATE TYPE core.tenant_status AS ENUM ('active', 'suspended', 'closed');
    END IF;

    IF to_regtype('core.organization_type') IS NULL THEN
        CREATE TYPE core.organization_type AS ENUM (
            'company',
            'branch',
            'department',
            'team',
            'service_center'
        );
    END IF;

    IF to_regtype('core.organization_status') IS NULL THEN
        CREATE TYPE core.organization_status AS ENUM ('active', 'inactive', 'suspended');
    END IF;

    IF to_regtype('core.branch_status') IS NULL THEN
        CREATE TYPE core.branch_status AS ENUM ('active', 'inactive', 'suspended');
    END IF;

    IF to_regtype('core.region_status') IS NULL THEN
        CREATE TYPE core.region_status AS ENUM ('active', 'inactive');
    END IF;

    IF to_regtype('core.team_status') IS NULL THEN
        CREATE TYPE core.team_status AS ENUM ('active', 'inactive');
    END IF;

    IF to_regtype('core.user_status') IS NULL THEN
        CREATE TYPE core.user_status AS ENUM ('invited', 'active', 'inactive', 'locked', 'archived');
    END IF;
END $$;

-- =========================================================
-- COMMON UPDATED_AT TRIGGER
-- =========================================================

CREATE OR REPLACE FUNCTION core.set_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at = now();
    RETURN NEW;
END;
$$;

-- =========================================================
-- TENANTS
-- =========================================================

CREATE TABLE IF NOT EXISTS core.tenants (
    tenant_id           uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_code         varchar(50)  NOT NULL,
    tenant_name         varchar(200) NOT NULL,
    legal_name          varchar(250),
    status              core.tenant_status NOT NULL DEFAULT 'active',
    primary_country_code char(2),
    default_timezone    varchar(64)  NOT NULL DEFAULT 'UTC',
    settings            jsonb        NOT NULL DEFAULT '{}'::jsonb,
    created_at          timestamptz  NOT NULL DEFAULT now(),
    updated_at          timestamptz  NOT NULL DEFAULT now(),
    deleted_at          timestamptz,

    CONSTRAINT uq_tenants_tenant_code UNIQUE (tenant_code)
);

DROP TRIGGER IF EXISTS trg_tenants_updated_at ON core.tenants;
CREATE TRIGGER trg_tenants_updated_at
BEFORE UPDATE ON core.tenants
FOR EACH ROW EXECUTE FUNCTION core.set_updated_at();

-- =========================================================
-- ORGANIZATIONS
-- =========================================================

CREATE TABLE IF NOT EXISTS core.organizations (
    organization_id        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id              uuid NOT NULL,
    parent_organization_id uuid,
    organization_code      varchar(50)  NOT NULL,
    organization_name      varchar(200) NOT NULL,
    legal_name             varchar(250),
    organization_type      core.organization_type NOT NULL DEFAULT 'company',
    tax_id                 varchar(50),
    registration_number    varchar(100),
    status                 core.organization_status NOT NULL DEFAULT 'active',
    settings               jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at             timestamptz NOT NULL DEFAULT now(),
    updated_at             timestamptz NOT NULL DEFAULT now(),
    deleted_at             timestamptz,

    CONSTRAINT fk_organizations_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_organizations_parent
        FOREIGN KEY (parent_organization_id) REFERENCES core.organizations (organization_id)
        ON DELETE RESTRICT,

    CONSTRAINT uq_organizations_tenant_code
        UNIQUE (tenant_id, organization_code)
);

CREATE INDEX IF NOT EXISTS ix_organizations_tenant_id
    ON core.organizations (tenant_id);

CREATE INDEX IF NOT EXISTS ix_organizations_parent_organization_id
    ON core.organizations (parent_organization_id);

DROP TRIGGER IF EXISTS trg_organizations_updated_at ON core.organizations;
CREATE TRIGGER trg_organizations_updated_at
BEFORE UPDATE ON core.organizations
FOR EACH ROW EXECUTE FUNCTION core.set_updated_at();

-- =========================================================
-- BRANCHES
-- =========================================================

CREATE TABLE IF NOT EXISTS core.branches (
    branch_id        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id        uuid NOT NULL,
    organization_id  uuid NOT NULL,
    branch_code      varchar(50)  NOT NULL,
    branch_name      varchar(200) NOT NULL,
    state_code       char(2),
    city             varchar(100),
    address_line1    varchar(200),
    address_line2    varchar(200),
    postal_code      varchar(20),
    country_code     char(2),
    time_zone        varchar(64) NOT NULL DEFAULT 'UTC',
    status           core.branch_status NOT NULL DEFAULT 'active',
    settings         jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at       timestamptz NOT NULL DEFAULT now(),
    updated_at       timestamptz NOT NULL DEFAULT now(),
    deleted_at       timestamptz,

    CONSTRAINT fk_branches_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_branches_organization
        FOREIGN KEY (organization_id) REFERENCES core.organizations (organization_id)
        ON DELETE RESTRICT,

    CONSTRAINT uq_branches_tenant_code
        UNIQUE (tenant_id, branch_code)
);

CREATE INDEX IF NOT EXISTS ix_branches_tenant_id
    ON core.branches (tenant_id);

CREATE INDEX IF NOT EXISTS ix_branches_organization_id
    ON core.branches (organization_id);

CREATE INDEX IF NOT EXISTS ix_branches_state_code
    ON core.branches (state_code);

DROP TRIGGER IF EXISTS trg_branches_updated_at ON core.branches;
CREATE TRIGGER trg_branches_updated_at
BEFORE UPDATE ON core.branches
FOR EACH ROW EXECUTE FUNCTION core.set_updated_at();

-- =========================================================
-- REGIONS
-- =========================================================

CREATE TABLE IF NOT EXISTS core.regions (
    region_id      uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id      uuid NOT NULL,
    organization_id uuid NOT NULL,
    region_code    varchar(50)  NOT NULL,
    region_name    varchar(200) NOT NULL,
    country_code   char(2),
    state_code     char(2),
    description    text,
    status         core.region_status NOT NULL DEFAULT 'active',
    settings       jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at     timestamptz NOT NULL DEFAULT now(),
    updated_at     timestamptz NOT NULL DEFAULT now(),
    deleted_at     timestamptz,

    CONSTRAINT fk_regions_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_regions_organization
        FOREIGN KEY (organization_id) REFERENCES core.organizations (organization_id)
        ON DELETE RESTRICT,

    CONSTRAINT uq_regions_tenant_code
        UNIQUE (tenant_id, region_code)
);

CREATE INDEX IF NOT EXISTS ix_regions_tenant_id
    ON core.regions (tenant_id);

CREATE INDEX IF NOT EXISTS ix_regions_organization_id
    ON core.regions (organization_id);

CREATE INDEX IF NOT EXISTS ix_regions_state_code
    ON core.regions (state_code);

DROP TRIGGER IF EXISTS trg_regions_updated_at ON core.regions;
CREATE TRIGGER trg_regions_updated_at
BEFORE UPDATE ON core.regions
FOR EACH ROW EXECUTE FUNCTION core.set_updated_at();

-- =========================================================
-- TEAMS
-- =========================================================

CREATE TABLE IF NOT EXISTS core.teams (
    team_id        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id      uuid NOT NULL,
    branch_id      uuid NOT NULL,
    region_id      uuid,
    team_code      varchar(50)  NOT NULL,
    team_name      varchar(200) NOT NULL,
    team_type      varchar(50),
    status         core.team_status NOT NULL DEFAULT 'active',
    settings       jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at     timestamptz NOT NULL DEFAULT now(),
    updated_at     timestamptz NOT NULL DEFAULT now(),
    deleted_at     timestamptz,

    CONSTRAINT fk_teams_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_teams_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_teams_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE RESTRICT,

    CONSTRAINT uq_teams_tenant_code
        UNIQUE (tenant_id, team_code)
);

CREATE INDEX IF NOT EXISTS ix_teams_tenant_id
    ON core.teams (tenant_id);

CREATE INDEX IF NOT EXISTS ix_teams_branch_id
    ON core.teams (branch_id);

CREATE INDEX IF NOT EXISTS ix_teams_region_id
    ON core.teams (region_id);

DROP TRIGGER IF EXISTS trg_teams_updated_at ON core.teams;
CREATE TRIGGER trg_teams_updated_at
BEFORE UPDATE ON core.teams
FOR EACH ROW EXECUTE FUNCTION core.set_updated_at();

-- =========================================================
-- USERS
-- =========================================================

CREATE TABLE IF NOT EXISTS core.users (
    user_id         uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id       uuid NOT NULL,
    organization_id uuid,
    branch_id       uuid,
    team_id         uuid,
    user_code       varchar(50)  NOT NULL,
    email           citext       NOT NULL,
    phone           varchar(30),
    password_hash   text         NOT NULL,
    first_name      varchar(100) NOT NULL,
    last_name       varchar(100) NOT NULL,
    display_name    varchar(200),
    status          core.user_status NOT NULL DEFAULT 'invited',
    locale          varchar(20)  NOT NULL DEFAULT 'en-US',
    time_zone       varchar(64)  NOT NULL DEFAULT 'UTC',
    last_login_at   timestamptz,
    settings        jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at      timestamptz NOT NULL DEFAULT now(),
    updated_at      timestamptz NOT NULL DEFAULT now(),
    deleted_at      timestamptz,

    CONSTRAINT fk_users_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_users_organization
        FOREIGN KEY (organization_id) REFERENCES core.organizations (organization_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_users_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_users_team
        FOREIGN KEY (team_id) REFERENCES core.teams (team_id)
        ON DELETE RESTRICT,

    CONSTRAINT uq_users_tenant_code
        UNIQUE (tenant_id, user_code),

    CONSTRAINT uq_users_tenant_email
        UNIQUE (tenant_id, email)
);

ALTER TABLE core.users
    ADD COLUMN failed_login_attempts INTEGER     NOT NULL DEFAULT 0,
    ADD COLUMN locked_until_utc      TIMESTAMPTZ NULL;

CREATE INDEX ix_users_locked_until ON core.users (locked_until_utc)
    WHERE locked_until_utc IS NOT NULL;

CREATE INDEX IF NOT EXISTS ix_users_tenant_id
    ON core.users (tenant_id);

CREATE INDEX IF NOT EXISTS ix_users_organization_id
    ON core.users (organization_id);

CREATE INDEX IF NOT EXISTS ix_users_branch_id
    ON core.users (branch_id);

CREATE INDEX IF NOT EXISTS ix_users_team_id
    ON core.users (team_id);

CREATE INDEX IF NOT EXISTS ix_users_status
    ON core.users (status);

DROP TRIGGER IF EXISTS trg_users_updated_at ON core.users;
CREATE TRIGGER trg_users_updated_at
BEFORE UPDATE ON core.users
FOR EACH ROW EXECUTE FUNCTION core.set_updated_at();

-- =========================================================
-- ROLES
-- =========================================================

CREATE TABLE IF NOT EXISTS core.roles (
    role_id        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id      uuid NOT NULL,
    role_code      varchar(100) NOT NULL,
    role_name      varchar(200) NOT NULL,
    description    text,
    is_system      boolean NOT NULL DEFAULT false,
    is_active      boolean NOT NULL DEFAULT true,
    created_at     timestamptz NOT NULL DEFAULT now(),
    updated_at     timestamptz NOT NULL DEFAULT now(),
    deleted_at     timestamptz,

    CONSTRAINT fk_roles_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT uq_roles_tenant_code
        UNIQUE (tenant_id, role_code)
);

CREATE INDEX IF NOT EXISTS ix_roles_tenant_id
    ON core.roles (tenant_id);

CREATE INDEX IF NOT EXISTS ix_roles_is_active
    ON core.roles (is_active);

DROP TRIGGER IF EXISTS trg_roles_updated_at ON core.roles;
CREATE TRIGGER trg_roles_updated_at
BEFORE UPDATE ON core.roles
FOR EACH ROW EXECUTE FUNCTION core.set_updated_at();

-- =========================================================
-- PERMISSIONS
-- =========================================================

CREATE TABLE IF NOT EXISTS core.permissions (
    permission_id    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    permission_code  varchar(150) NOT NULL,
    module_name      varchar(100)  NOT NULL,
    permission_name  varchar(200)  NOT NULL,
    description      text,
    is_active        boolean NOT NULL DEFAULT true,
    created_at       timestamptz NOT NULL DEFAULT now(),
    updated_at       timestamptz NOT NULL DEFAULT now(),
    deleted_at       timestamptz,

    CONSTRAINT uq_permissions_code
        UNIQUE (permission_code)
);

CREATE INDEX IF NOT EXISTS ix_permissions_module_name
    ON core.permissions (module_name);

CREATE INDEX IF NOT EXISTS ix_permissions_is_active
    ON core.permissions (is_active);

DROP TRIGGER IF EXISTS trg_permissions_updated_at ON core.permissions;
CREATE TRIGGER trg_permissions_updated_at
BEFORE UPDATE ON core.permissions
FOR EACH ROW EXECUTE FUNCTION core.set_updated_at();

-- =========================================================
-- USER_ROLES
-- =========================================================

CREATE TABLE IF NOT EXISTS core.user_roles (
    user_id            uuid NOT NULL,
    role_id            uuid NOT NULL,
    assigned_by_user_id uuid,
    assigned_at        timestamptz NOT NULL DEFAULT now(),

    PRIMARY KEY (user_id, role_id),

    CONSTRAINT fk_user_roles_user
        FOREIGN KEY (user_id) REFERENCES core.users (user_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_user_roles_role
        FOREIGN KEY (role_id) REFERENCES core.roles (role_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_user_roles_assigned_by_user
        FOREIGN KEY (assigned_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_user_roles_role_id
    ON core.user_roles (role_id);

CREATE INDEX IF NOT EXISTS ix_user_roles_assigned_by_user_id
    ON core.user_roles (assigned_by_user_id);

-- =========================================================
-- ROLE_PERMISSIONS
-- =========================================================

CREATE TABLE IF NOT EXISTS core.role_permissions (
    role_id        uuid NOT NULL,
    permission_id  uuid NOT NULL,
    granted_at     timestamptz NOT NULL DEFAULT now(),

    PRIMARY KEY (role_id, permission_id),

    CONSTRAINT fk_role_permissions_role
        FOREIGN KEY (role_id) REFERENCES core.roles (role_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_role_permissions_permission
        FOREIGN KEY (permission_id) REFERENCES core.permissions (permission_id)
        ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS ix_role_permissions_permission_id
    ON core.role_permissions (permission_id);

-- =========================================================
-- REFRESH TOKENS / USER SESSIONS
-- =========================================================

CREATE TABLE IF NOT EXISTS core.refresh_tokens (
    refresh_token_id      uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id             uuid NOT NULL,
    user_id               uuid NOT NULL,
    token_hash            char(64) NOT NULL,
    device_name           varchar(200),
    user_agent            text,
    created_ip            inet,
    expires_at            timestamptz NOT NULL,
    last_used_at          timestamptz,
    revoked_at            timestamptz,
    replaced_by_token_id  uuid,
    created_at            timestamptz NOT NULL DEFAULT now(),
    updated_at            timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_refresh_tokens_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_refresh_tokens_user
        FOREIGN KEY (user_id) REFERENCES core.users (user_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_refresh_tokens_replaced_by
        FOREIGN KEY (replaced_by_token_id) REFERENCES core.refresh_tokens (refresh_token_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_refresh_tokens_token_hash
        UNIQUE (token_hash)
);
DROP TABLE core.password_reset_tokens
CREATE TABLE core.password_reset_tokens (
    password_reset_token_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),

    tenant_id uuid NOT NULL,
    user_id uuid NOT NULL,
    token_hash char(64) NOT NULL,
    expires_at timestamptz NOT NULL,
    used_at timestamptz NULL,

    created_by_user_id uuid NULL,
    created_ip inet NULL,

    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT uq_password_reset_tokens_token_hash
        UNIQUE (token_hash),

    CONSTRAINT fk_password_reset_tokens_tenant
        FOREIGN KEY (tenant_id)
        REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_password_reset_tokens_user
        FOREIGN KEY (user_id)
        REFERENCES core.users (user_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_password_reset_tokens_created_by_user
        FOREIGN KEY (created_by_user_id)
        REFERENCES core.users (user_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_refresh_tokens_tenant_id
    ON core.refresh_tokens (tenant_id);

CREATE INDEX IF NOT EXISTS ix_refresh_tokens_user_id
    ON core.refresh_tokens (user_id);

CREATE INDEX IF NOT EXISTS ix_refresh_tokens_expires_at
    ON core.refresh_tokens (expires_at);

CREATE INDEX IF NOT EXISTS ix_refresh_tokens_revoked_at
    ON core.refresh_tokens (revoked_at);

CREATE INDEX IF NOT EXISTS ix_password_reset_tokens_user_id    ON core.password_reset_tokens(user_id);
CREATE INDEX IF NOT EXISTS ix_password_reset_tokens_expires_at ON core.password_reset_tokens(expires_at);
CREATE INDEX IF NOT EXISTS ix_password_reset_tokens_used_at    ON core.password_reset_tokens(used_at);

DROP TRIGGER IF EXISTS trg_password_reset_tokens_updated_at ON core.password_reset_tokens;
CREATE TRIGGER trg_password_reset_tokens_updated_at
    BEFORE UPDATE ON core.password_reset_tokens
    FOR EACH ROW EXECUTE FUNCTION core.set_updated_at();

DROP TRIGGER IF EXISTS trg_refresh_tokens_updated_at ON core.refresh_tokens;
CREATE TRIGGER trg_refresh_tokens_updated_at
BEFORE UPDATE ON core.refresh_tokens
FOR EACH ROW EXECUTE FUNCTION core.set_updated_at();

-- =========================================================
-- OPTIONAL VIEW: ACTIVE USERS IN A TENANT
-- =========================================================

CREATE OR REPLACE VIEW core.v_active_users AS
SELECT
    u.user_id,
    u.tenant_id,
    u.organization_id,
    u.branch_id,
    u.team_id,
    u.user_code,
    u.email,
    u.phone,
    u.first_name,
    u.last_name,
    u.display_name,
    u.status,
    u.locale,
    u.time_zone,
    u.last_login_at,
    u.created_at,
    u.updated_at
FROM core.users u
WHERE u.deleted_at IS NULL
  AND u.status = 'active';

-- =========================================================
-- OPTIONAL COMMENTARY
-- =========================================================

COMMENT ON SCHEMA core IS 'Core schema for tenant, organization, user, role, permission, and session management.';
COMMENT ON TABLE core.tenants IS 'Tenant root record for multi-tenant isolation.';
COMMENT ON TABLE core.organizations IS 'Company-level organizational structure.';
COMMENT ON TABLE core.branches IS 'Operational branches/offices.';
COMMENT ON TABLE core.regions IS 'Service regions used for dispatch and compliance.';
COMMENT ON TABLE core.teams IS 'Work teams under branches/regions.';
COMMENT ON TABLE core.users IS 'Application users and staff accounts.';
COMMENT ON TABLE core.roles IS 'Tenant-scoped roles.';
COMMENT ON TABLE core.permissions IS 'Global permission catalog.';
COMMENT ON TABLE core.user_roles IS 'Many-to-many mapping between users and roles.';
COMMENT ON TABLE core.role_permissions IS 'Many-to-many mapping between roles and permissions.';
COMMENT ON TABLE core.refresh_tokens IS 'Refresh token/session tracking for authentication.';




-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================

-- PostgreSQL 15+
-- Identity schema for Notary profile, commission, bond, insurance, capability, document, and history management

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

CREATE SCHEMA IF NOT EXISTS identity;

-- =========================================================
-- ENUM TYPES
-- =========================================================

DO $$
BEGIN
    IF to_regtype('identity.notary_status') IS NULL THEN
        CREATE TYPE identity.notary_status AS ENUM (
            'active',
            'inactive',
            'suspended',
            'blocked',
            'expired',
            'pending'
        );
    END IF;

    IF to_regtype('identity.commission_status') IS NULL THEN
        CREATE TYPE identity.commission_status AS ENUM (
            'pending',
            'active',
            'expiring',
            'expired',
            'revoked',
            'suspended'
        );
    END IF;

    IF to_regtype('identity.bond_status') IS NULL THEN
        CREATE TYPE identity.bond_status AS ENUM (
            'valid',
            'expiring',
            'expired',
            'missing',
            'cancelled'
        );
    END IF;

    IF to_regtype('identity.insurance_status') IS NULL THEN
        CREATE TYPE identity.insurance_status AS ENUM (
            'valid',
            'expiring',
            'expired',
            'missing',
            'cancelled'
        );
    END IF;

    IF to_regtype('identity.document_status') IS NULL THEN
        CREATE TYPE identity.document_status AS ENUM (
            'uploaded',
            'verified',
            'rejected',
            'expired',
            'archived'
        );
    END IF;

    IF to_regtype('identity.capability_code') IS NULL THEN
        CREATE TYPE identity.capability_code AS ENUM (
            'acknowledgment',
            'jurat',
            'copy_certification',
            'mobile_notary',
            'ron',
            'loan_signing',
            'apostille_support'
        );
    END IF;

    IF to_regtype('identity.history_action_type') IS NULL THEN
        CREATE TYPE identity.history_action_type AS ENUM (
            'create',
            'update',
            'activate',
            'suspend',
            'reactivate',
            'expire',
            'revoke',
            'block',
            'unlock',
            'upload_document',
            'verify_document',
            'reject_document'
        );
    END IF;
END $$;

-- =========================================================
-- COMMON UPDATED_AT TRIGGER
-- =========================================================

CREATE OR REPLACE FUNCTION identity.set_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at = now();
    RETURN NEW;
END;
$$;

-- =========================================================
-- NOTARIES
-- One profile per user (usually)
-- =========================================================

CREATE TABLE IF NOT EXISTS identity.notaries (
    notary_id                uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    user_id                  uuid NOT NULL,
    internal_notary_code     varchar(50) NOT NULL,
    public_display_name      varchar(200),
    status                   identity.notary_status NOT NULL DEFAULT 'pending',

    commissioning_state_code  char(2) NOT NULL,
    commission_number        varchar(100),
    commission_issue_date     date,
    commission_expiration_date date,

    employment_type          varchar(50),   -- Employee / Independent Contractor
    start_date               date,
    branch_id                uuid,
    region_id                uuid,

    error_rate               numeric(5,2) NOT NULL DEFAULT 0,
    customer_rating          numeric(3,2) NOT NULL DEFAULT 0,
    total_jobs_completed     integer NOT NULL DEFAULT 0,

    notes                    text,
    settings                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_notaries_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_notaries_user
        FOREIGN KEY (user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_notaries_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_notaries_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_notaries_user
        UNIQUE (user_id),

    CONSTRAINT uq_notaries_tenant_code
        UNIQUE (tenant_id, internal_notary_code),

    CONSTRAINT chk_notaries_rating
        CHECK (customer_rating >= 0 AND customer_rating <= 5),

    CONSTRAINT chk_notaries_error_rate
        CHECK (error_rate >= 0 AND error_rate <= 100)
);

CREATE INDEX IF NOT EXISTS ix_notaries_tenant_id
    ON identity.notaries (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notaries_user_id
    ON identity.notaries (user_id);

CREATE INDEX IF NOT EXISTS ix_notaries_status
    ON identity.notaries (status);

CREATE INDEX IF NOT EXISTS ix_notaries_commissioning_state_code
    ON identity.notaries (commissioning_state_code);

CREATE INDEX IF NOT EXISTS ix_notaries_branch_id
    ON identity.notaries (branch_id);

CREATE INDEX IF NOT EXISTS ix_notaries_region_id
    ON identity.notaries (region_id);

DROP TRIGGER IF EXISTS trg_notaries_updated_at ON identity.notaries;
CREATE TRIGGER trg_notaries_updated_at
BEFORE UPDATE ON identity.notaries
FOR EACH ROW EXECUTE FUNCTION identity.set_updated_at();

-- =========================================================
-- NOTARY COMMISSIONS
-- =========================================================

CREATE TABLE IF NOT EXISTS identity.notary_commissions (
    commission_id            uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    notary_id                uuid NOT NULL,

    commission_number        varchar(100) NOT NULL,
    commissioning_state_code  char(2) NOT NULL,
    issue_date               date NOT NULL,
    expiration_date          date NOT NULL,
    status                   identity.commission_status NOT NULL DEFAULT 'active',

    renewal_submitted        boolean NOT NULL DEFAULT false,
    renewal_submitted_at     timestamptz,
    expected_renewal_date    date,

    revoked_at               timestamptz,
    revoked_reason           text,

    source_document_id       uuid,
    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_commissions_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_commissions_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE CASCADE,

    CONSTRAINT uq_commissions_notary_number
        UNIQUE (notary_id, commission_number),

    CONSTRAINT chk_commissions_dates
        CHECK (expiration_date > issue_date)
);

CREATE INDEX IF NOT EXISTS ix_notary_commissions_tenant_id
    ON identity.notary_commissions (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notary_commissions_notary_id
    ON identity.notary_commissions (notary_id);

CREATE INDEX IF NOT EXISTS ix_notary_commissions_status
    ON identity.notary_commissions (status);

CREATE INDEX IF NOT EXISTS ix_notary_commissions_expiration_date
    ON identity.notary_commissions (expiration_date);

DROP TRIGGER IF EXISTS trg_notary_commissions_updated_at ON identity.notary_commissions;
CREATE TRIGGER trg_notary_commissions_updated_at
BEFORE UPDATE ON identity.notary_commissions
FOR EACH ROW EXECUTE FUNCTION identity.set_updated_at();

-- =========================================================
-- NOTARY LICENSES / CERTIFICATIONS
-- Commission certificates, training certs, identity docs, etc.
-- =========================================================

CREATE TABLE IF NOT EXISTS identity.notary_licenses (
    license_id               uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    notary_id                uuid NOT NULL,

    license_type             varchar(100) NOT NULL,
    license_number           varchar(100),
    issuing_authority        varchar(200),
    issue_date               date,
    expiration_date          date,
    status                   identity.document_status NOT NULL DEFAULT 'uploaded',

    verification_status      varchar(50) NOT NULL DEFAULT 'pending',
    verified_by_user_id      uuid,
    verified_at              timestamptz,
    rejection_reason         text,

    source_document_id       uuid,
    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_licenses_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_licenses_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_licenses_verified_by
        FOREIGN KEY (verified_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_notary_licenses_tenant_id
    ON identity.notary_licenses (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notary_licenses_notary_id
    ON identity.notary_licenses (notary_id);

CREATE INDEX IF NOT EXISTS ix_notary_licenses_status
    ON identity.notary_licenses (status);

CREATE INDEX IF NOT EXISTS ix_notary_licenses_expiration_date
    ON identity.notary_licenses (expiration_date);

DROP TRIGGER IF EXISTS trg_notary_licenses_updated_at ON identity.notary_licenses;
CREATE TRIGGER trg_notary_licenses_updated_at
BEFORE UPDATE ON identity.notary_licenses
FOR EACH ROW EXECUTE FUNCTION identity.set_updated_at();

-- =========================================================
-- NOTARY BONDS
-- =========================================================

CREATE TABLE IF NOT EXISTS identity.notary_bonds (
    bond_id                  uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    notary_id                uuid NOT NULL,

    provider_name            varchar(200) NOT NULL,
    bond_number              varchar(100),
    bond_amount              numeric(18,2) NOT NULL DEFAULT 0,
    effective_date           date NOT NULL,
    expiration_date          date NOT NULL,
    status                   identity.bond_status NOT NULL DEFAULT 'valid',

    document_file_id         uuid,
    notes                    text,
    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_bonds_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_bonds_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE CASCADE,

    CONSTRAINT chk_bonds_dates
        CHECK (expiration_date > effective_date),

    CONSTRAINT chk_bonds_amount
        CHECK (bond_amount >= 0)
);

CREATE INDEX IF NOT EXISTS ix_notary_bonds_tenant_id
    ON identity.notary_bonds (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notary_bonds_notary_id
    ON identity.notary_bonds (notary_id);

CREATE INDEX IF NOT EXISTS ix_notary_bonds_status
    ON identity.notary_bonds (status);

CREATE INDEX IF NOT EXISTS ix_notary_bonds_expiration_date
    ON identity.notary_bonds (expiration_date);

DROP TRIGGER IF EXISTS trg_notary_bonds_updated_at ON identity.notary_bonds;
CREATE TRIGGER trg_notary_bonds_updated_at
BEFORE UPDATE ON identity.notary_bonds
FOR EACH ROW EXECUTE FUNCTION identity.set_updated_at();

-- =========================================================
-- NOTARY INSURANCES (E&O)
-- =========================================================

CREATE TABLE IF NOT EXISTS identity.notary_insurances (
    insurance_id             uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    notary_id                uuid NOT NULL,

    provider_name            varchar(200) NOT NULL,
    policy_number            varchar(100),
    coverage_amount          numeric(18,2) NOT NULL DEFAULT 0,
    effective_date           date NOT NULL,
    expiration_date          date NOT NULL,
    status                   identity.insurance_status NOT NULL DEFAULT 'valid',

    document_file_id         uuid,
    notes                    text,
    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_insurances_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_insurances_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE CASCADE,

    CONSTRAINT chk_insurances_dates
        CHECK (expiration_date > effective_date),

    CONSTRAINT chk_insurances_coverage
        CHECK (coverage_amount >= 0)
);

CREATE INDEX IF NOT EXISTS ix_notary_insurances_tenant_id
    ON identity.notary_insurances (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notary_insurances_notary_id
    ON identity.notary_insurances (notary_id);

CREATE INDEX IF NOT EXISTS ix_notary_insurances_status
    ON identity.notary_insurances (status);

CREATE INDEX IF NOT EXISTS ix_notary_insurances_expiration_date
    ON identity.notary_insurances (expiration_date);

DROP TRIGGER IF EXISTS trg_notary_insurances_updated_at ON identity.notary_insurances;
CREATE TRIGGER trg_notary_insurances_updated_at
BEFORE UPDATE ON identity.notary_insurances
FOR EACH ROW EXECUTE FUNCTION identity.set_updated_at();

-- =========================================================
-- NOTARY CAPABILITIES
-- One row per capability per notary
-- =========================================================

CREATE TABLE IF NOT EXISTS identity.notary_capabilities (
    notary_capability_id     uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    notary_id                uuid NOT NULL,
    capability               identity.capability_code NOT NULL,
    is_authorized            boolean NOT NULL DEFAULT true,

    authorized_state_code    char(2),
    county_name              varchar(100),
    zip_code                 varchar(20),

    valid_from               date,
    valid_to                 date,
    notes                    text,
    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_capabilities_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_capabilities_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE CASCADE,

    CONSTRAINT uq_notary_capability
        UNIQUE (notary_id, capability, authorized_state_code, county_name, zip_code)
);

CREATE INDEX IF NOT EXISTS ix_notary_capabilities_tenant_id
    ON identity.notary_capabilities (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notary_capabilities_notary_id
    ON identity.notary_capabilities (notary_id);

CREATE INDEX IF NOT EXISTS ix_notary_capabilities_capability
    ON identity.notary_capabilities (capability);

DROP TRIGGER IF EXISTS trg_notary_capabilities_updated_at ON identity.notary_capabilities;
CREATE TRIGGER trg_notary_capabilities_updated_at
BEFORE UPDATE ON identity.notary_capabilities
FOR EACH ROW EXECUTE FUNCTION identity.set_updated_at();

-- =========================================================
-- NOTARY DOCUMENTS
-- Commission certificates, training certs, ID docs, etc.
-- =========================================================

CREATE TABLE IF NOT EXISTS identity.notary_documents (
    document_id              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    notary_id                uuid NOT NULL,

    document_category        varchar(100) NOT NULL,
    document_type            varchar(100) NOT NULL,
    file_name                varchar(255) NOT NULL,
    file_extension           varchar(20),
    mime_type                varchar(100),
    storage_provider         varchar(50) NOT NULL DEFAULT 'object_storage',
    storage_key              text NOT NULL,
    file_size_bytes          bigint,
    checksum_sha256          char(64),

    issue_date               date,
    expiration_date          date,
    status                   identity.document_status NOT NULL DEFAULT 'uploaded',

    is_sensitive             boolean NOT NULL DEFAULT false,
    visibility_level         varchar(50) NOT NULL DEFAULT 'restricted',

    uploaded_by_user_id      uuid,
    uploaded_at              timestamptz NOT NULL DEFAULT now(),
    verified_by_user_id      uuid,
    verified_at              timestamptz,
    verification_notes       text,

    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_documents_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_documents_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_documents_uploaded_by
        FOREIGN KEY (uploaded_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_documents_verified_by
        FOREIGN KEY (verified_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_notary_documents_tenant_id
    ON identity.notary_documents (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notary_documents_notary_id
    ON identity.notary_documents (notary_id);

CREATE INDEX IF NOT EXISTS ix_notary_documents_category
    ON identity.notary_documents (document_category);

CREATE INDEX IF NOT EXISTS ix_notary_documents_type
    ON identity.notary_documents (document_type);

CREATE INDEX IF NOT EXISTS ix_notary_documents_status
    ON identity.notary_documents (status);

CREATE INDEX IF NOT EXISTS ix_notary_documents_expiration_date
    ON identity.notary_documents (expiration_date);

DROP TRIGGER IF EXISTS trg_notary_documents_updated_at ON identity.notary_documents;
CREATE TRIGGER trg_notary_documents_updated_at
BEFORE UPDATE ON identity.notary_documents
FOR EACH ROW EXECUTE FUNCTION identity.set_updated_at();

-- =========================================================
-- NOTARY STATUS HISTORY
-- =========================================================

CREATE TABLE IF NOT EXISTS identity.notary_status_history (
    status_history_id        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    notary_id                uuid NOT NULL,

    previous_status          identity.notary_status,
    new_status               identity.notary_status NOT NULL,
    action_type              identity.history_action_type NOT NULL,
    reason                   text,

    effective_at             timestamptz NOT NULL DEFAULT now(),
    performed_by_user_id     uuid,
    source_reference         text,
    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at               timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_status_history_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_status_history_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_status_history_performed_by
        FOREIGN KEY (performed_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_notary_status_history_tenant_id
    ON identity.notary_status_history (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notary_status_history_notary_id
    ON identity.notary_status_history (notary_id);

CREATE INDEX IF NOT EXISTS ix_notary_status_history_effective_at
    ON identity.notary_status_history (effective_at);

CREATE INDEX IF NOT EXISTS ix_notary_status_history_new_status
    ON identity.notary_status_history (new_status);

-- =========================================================
-- NOTARY AUDIT NOTES
-- Human-readable audit notes for senior admins/compliance
-- =========================================================

CREATE TABLE IF NOT EXISTS identity.notary_audit_notes (
    audit_note_id            uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    notary_id                uuid NOT NULL,

    note_title               varchar(200),
    note_body                text NOT NULL,
    visibility_level         varchar(50) NOT NULL DEFAULT 'restricted',

    created_by_user_id       uuid NOT NULL,
    updated_by_user_id       uuid,
    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_audit_notes_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_audit_notes_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_audit_notes_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_audit_notes_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_notary_audit_notes_tenant_id
    ON identity.notary_audit_notes (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notary_audit_notes_notary_id
    ON identity.notary_audit_notes (notary_id);

CREATE INDEX IF NOT EXISTS ix_notary_audit_notes_created_at
    ON identity.notary_audit_notes (created_at);

DROP TRIGGER IF EXISTS trg_notary_audit_notes_updated_at ON identity.notary_audit_notes;
CREATE TRIGGER trg_notary_audit_notes_updated_at
BEFORE UPDATE ON identity.notary_audit_notes
FOR EACH ROW EXECUTE FUNCTION identity.set_updated_at();

-- =========================================================
-- OPTIONAL VIEWS
-- =========================================================

CREATE OR REPLACE VIEW identity.v_notary_profile_overview AS
SELECT
    n.notary_id,
    n.tenant_id,
    n.user_id,
    n.internal_notary_code,
    n.public_display_name,
    n.status,
    n.commissioning_state_code,
    n.commission_number,
    n.commission_issue_date,
    n.commission_expiration_date,
    n.employment_type,
    n.start_date,
    n.error_rate,
    n.customer_rating,
    n.total_jobs_completed,
    u.email,
    u.phone,
    u.first_name,
    u.last_name,
    u.display_name,
    u.status AS user_status
FROM identity.notaries n
JOIN core.users u ON u.user_id = n.user_id
WHERE n.deleted_at IS NULL
  AND u.deleted_at IS NULL;

CREATE OR REPLACE VIEW identity.v_notary_compliance_status AS
SELECT
    n.notary_id,
    n.tenant_id,
    n.public_display_name,
    n.status AS notary_status,
    c.status AS commission_status,
    c.expiration_date AS commission_expiration_date,
    b.status AS bond_status,
    b.expiration_date AS bond_expiration_date,
    i.status AS insurance_status,
    i.expiration_date AS insurance_expiration_date
FROM identity.notaries n
LEFT JOIN LATERAL (
    SELECT nc.status, nc.expiration_date
    FROM identity.notary_commissions nc
    WHERE nc.notary_id = n.notary_id
      AND nc.deleted_at IS NULL
    ORDER BY nc.created_at DESC
    LIMIT 1
) c ON TRUE
LEFT JOIN LATERAL (
    SELECT nb.status, nb.expiration_date
    FROM identity.notary_bonds nb
    WHERE nb.notary_id = n.notary_id
      AND nb.deleted_at IS NULL
    ORDER BY nb.created_at DESC
    LIMIT 1
) b ON TRUE
LEFT JOIN LATERAL (
    SELECT ni.status, ni.expiration_date
    FROM identity.notary_insurances ni
    WHERE ni.notary_id = n.notary_id
      AND ni.deleted_at IS NULL
    ORDER BY ni.created_at DESC
    LIMIT 1
) i ON TRUE
WHERE n.deleted_at IS NULL;

-- =========================================================
-- COMMENTS
-- =========================================================

COMMENT ON SCHEMA identity IS 'Identity and notary profile schema.';
COMMENT ON TABLE identity.notaries IS 'Central notary profile linked to application user.';
COMMENT ON TABLE identity.notary_commissions IS 'Commission history and current commission state.';
COMMENT ON TABLE identity.notary_licenses IS 'License, certification, and verification records.';
COMMENT ON TABLE identity.notary_bonds IS 'Surety bond records.';
COMMENT ON TABLE identity.notary_insurances IS 'Errors and Omissions insurance records.';
COMMENT ON TABLE identity.notary_capabilities IS 'Notary service capabilities and jurisdiction scope.';
COMMENT ON TABLE identity.notary_documents IS 'Stored notary-related documents and files.';
COMMENT ON TABLE identity.notary_status_history IS 'Status transition history for compliance and audit.';
COMMENT ON TABLE identity.notary_audit_notes IS 'Human-readable notes for senior admins and compliance.';

-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================

-- PostgreSQL 15+
-- CRM schema for customer/account, contacts, tags, segments, notes, documents,
-- pricing, contracts, and SLA management.

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

CREATE SCHEMA IF NOT EXISTS crm;

-- =========================================================
-- ENUM TYPES
-- =========================================================

DO $$
BEGIN
    IF to_regtype('crm.customer_type') IS NULL THEN
        CREATE TYPE crm.customer_type AS ENUM ('individual', 'company');
    END IF;

    IF to_regtype('crm.customer_status') IS NULL THEN
        CREATE TYPE crm.customer_status AS ENUM (
            'active',
            'inactive',
            'suspended',
            'archived'
        );
    END IF;

    IF to_regtype('crm.contact_status') IS NULL THEN
        CREATE TYPE crm.contact_status AS ENUM (
            'active',
            'inactive',
            'archived'
        );
    END IF;

    IF to_regtype('crm.contact_role') IS NULL THEN
        CREATE TYPE crm.contact_role AS ENUM (
            'primary',
            'billing',
            'ordering',
            'legal',
            'technical',
            'other'
        );
    END IF;

    IF to_regtype('crm.segment_type') IS NULL THEN
        CREATE TYPE crm.segment_type AS ENUM (
            'vip',
            'high_volume',
            'preferred',
            'risk',
            'custom'
        );
    END IF;

    IF to_regtype('crm.document_status') IS NULL THEN
        CREATE TYPE crm.document_status AS ENUM (
            'uploaded',
            'verified',
            'rejected',
            'archived'
        );
    END IF;

    IF to_regtype('crm.pricing_model') IS NULL THEN
        CREATE TYPE crm.pricing_model AS ENUM (
            'fixed',
            'volume_based',
            'tiered',
            'per_job',
            'custom'
        );
    END IF;

    IF to_regtype('crm.pricing_rule_type') IS NULL THEN
        CREATE TYPE crm.pricing_rule_type AS ENUM (
            'base_rate',
            'volume_tier',
            'discount',
            'surcharge',
            'custom_exception'
        );
    END IF;

    IF to_regtype('crm.contract_status') IS NULL THEN
        CREATE TYPE crm.contract_status AS ENUM (
            'draft',
            'active',
            'expired',
            'terminated',
            'renewed',
            'cancelled'
        );
    END IF;

    IF to_regtype('crm.sla_status') IS NULL THEN
        CREATE TYPE crm.sla_status AS ENUM (
            'draft',
            'active',
            'expired',
            'breached',
            'terminated'
        );
    END IF;

    IF to_regtype('crm.note_visibility') IS NULL THEN
        CREATE TYPE crm.note_visibility AS ENUM (
            'private',
            'team',
            'company'
        );
    END IF;
END $$;

-- =========================================================
-- COMMON UPDATED_AT TRIGGER
-- =========================================================

CREATE OR REPLACE FUNCTION crm.set_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at = now();
    RETURN NEW;
END;
$$;

-- =========================================================
-- CUSTOMERS
-- Central account/entity for both B2C and B2B
-- =========================================================

CREATE TABLE IF NOT EXISTS crm.customers (
    customer_id            uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id              uuid NOT NULL,

    customer_code          varchar(50)  NOT NULL,
    customer_type          crm.customer_type NOT NULL,
    status                 crm.customer_status NOT NULL DEFAULT 'active',

    display_name           varchar(200) NOT NULL,
    legal_name             varchar(250),
    tax_id                 varchar(50),
    industry               varchar(120),
    website                varchar(250),

    primary_email          citext,
    primary_phone          varchar(30),
    billing_email          citext,
    billing_phone          varchar(30),

    account_manager_user_id uuid,
    branch_id              uuid,
    region_id              uuid,

    address_line1          varchar(200),
    address_line2          varchar(200),
    city                   varchar(100),
    state_code             char(2),
    postal_code            varchar(20),
    country_code           char(2),

    total_jobs             integer NOT NULL DEFAULT 0,
    total_revenue          numeric(18,2) NOT NULL DEFAULT 0,
    avg_turnaround_minutes integer,

    tags_summary           jsonb NOT NULL DEFAULT '[]'::jsonb,
    settings               jsonb NOT NULL DEFAULT '{}'::jsonb,
    notes_summary          text,

    created_at             timestamptz NOT NULL DEFAULT now(),
    updated_at             timestamptz NOT NULL DEFAULT now(),
    deleted_at             timestamptz,

    CONSTRAINT fk_customers_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_customers_account_manager
        FOREIGN KEY (account_manager_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_customers_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_customers_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_customers_tenant_code
        UNIQUE (tenant_id, customer_code),

    CONSTRAINT chk_customers_total_jobs
        CHECK (total_jobs >= 0),

    CONSTRAINT chk_customers_total_revenue
        CHECK (total_revenue >= 0)
);

CREATE INDEX IF NOT EXISTS ix_customers_tenant_id
    ON crm.customers (tenant_id);

CREATE INDEX IF NOT EXISTS ix_customers_status
    ON crm.customers (status);

CREATE INDEX IF NOT EXISTS ix_customers_customer_type
    ON crm.customers (customer_type);

CREATE INDEX IF NOT EXISTS ix_customers_account_manager_user_id
    ON crm.customers (account_manager_user_id);

CREATE INDEX IF NOT EXISTS ix_customers_branch_id
    ON crm.customers (branch_id);

CREATE INDEX IF NOT EXISTS ix_customers_region_id
    ON crm.customers (region_id);

CREATE INDEX IF NOT EXISTS ix_customers_legal_name
    ON crm.customers (legal_name);

DROP TRIGGER IF EXISTS trg_customers_updated_at ON crm.customers;
CREATE TRIGGER trg_customers_updated_at
BEFORE UPDATE ON crm.customers
FOR EACH ROW EXECUTE FUNCTION crm.set_updated_at();

-- =========================================================
-- CUSTOMER CONTACTS
-- Multiple contacts per B2B customer/account
-- =========================================================

CREATE TABLE IF NOT EXISTS crm.customer_contacts (
    contact_id         uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id          uuid NOT NULL,
    customer_id        uuid NOT NULL,

    contact_code       varchar(50) NOT NULL,
    full_name          varchar(200) NOT NULL,
    job_title          varchar(150),
    role               crm.contact_role NOT NULL DEFAULT 'other',
    status             crm.contact_status NOT NULL DEFAULT 'active',

    email              citext,
    phone              varchar(30),
    mobile_phone       varchar(30),
    department         varchar(120),

    is_primary         boolean NOT NULL DEFAULT false,
    notes              text,
    settings           jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at         timestamptz NOT NULL DEFAULT now(),
    updated_at         timestamptz NOT NULL DEFAULT now(),
    deleted_at         timestamptz,

    CONSTRAINT fk_customer_contacts_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_customer_contacts_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE CASCADE,

    CONSTRAINT uq_customer_contacts_customer_code
        UNIQUE (customer_id, contact_code),

    CONSTRAINT uq_customer_contacts_customer_email
        UNIQUE (customer_id, email)
);

CREATE INDEX IF NOT EXISTS ix_customer_contacts_tenant_id
    ON crm.customer_contacts (tenant_id);

CREATE INDEX IF NOT EXISTS ix_customer_contacts_customer_id
    ON crm.customer_contacts (customer_id);

CREATE INDEX IF NOT EXISTS ix_customer_contacts_role
    ON crm.customer_contacts (role);

CREATE INDEX IF NOT EXISTS ix_customer_contacts_status
    ON crm.customer_contacts (status);

CREATE INDEX IF NOT EXISTS ix_customer_contacts_is_primary
    ON crm.customer_contacts (is_primary);

DROP TRIGGER IF EXISTS trg_customer_contacts_updated_at ON crm.customer_contacts;
CREATE TRIGGER trg_customer_contacts_updated_at
BEFORE UPDATE ON crm.customer_contacts
FOR EACH ROW EXECUTE FUNCTION crm.set_updated_at();

-- Ensure only one primary contact per customer
CREATE UNIQUE INDEX IF NOT EXISTS ux_customer_contacts_one_primary
    ON crm.customer_contacts (customer_id)
    WHERE is_primary = true AND deleted_at IS NULL;

-- =========================================================
-- CONTACT NOTIFICATION PREFERENCES
-- =========================================================

CREATE TABLE IF NOT EXISTS crm.customer_contact_preferences (
    contact_id              uuid PRIMARY KEY,
    tenant_id               uuid NOT NULL,

    email_enabled           boolean NOT NULL DEFAULT true,
    sms_enabled             boolean NOT NULL DEFAULT false,
    in_app_enabled          boolean NOT NULL DEFAULT true,
    marketing_enabled       boolean NOT NULL DEFAULT false,
    operational_enabled     boolean NOT NULL DEFAULT true,
    billing_enabled         boolean NOT NULL DEFAULT true,

    preferred_language      varchar(20) NOT NULL DEFAULT 'en',
    quiet_hours_start       time,
    quiet_hours_end         time,

    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_contact_preferences_contact
        FOREIGN KEY (contact_id) REFERENCES crm.customer_contacts (contact_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_contact_preferences_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS ix_customer_contact_preferences_tenant_id
    ON crm.customer_contact_preferences (tenant_id);

DROP TRIGGER IF EXISTS trg_customer_contact_preferences_updated_at ON crm.customer_contact_preferences;
CREATE TRIGGER trg_customer_contact_preferences_updated_at
BEFORE UPDATE ON crm.customer_contact_preferences
FOR EACH ROW EXECUTE FUNCTION crm.set_updated_at();

-- =========================================================
-- TAGS
-- =========================================================

CREATE TABLE IF NOT EXISTS crm.customer_tags (
    tag_id          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id       uuid NOT NULL,
    tag_code        varchar(50)  NOT NULL,
    tag_name        varchar(100) NOT NULL,
    description     text,
    color_code      varchar(20),
    is_system       boolean NOT NULL DEFAULT false,
    is_active       boolean NOT NULL DEFAULT true,
    created_at      timestamptz NOT NULL DEFAULT now(),
    updated_at      timestamptz NOT NULL DEFAULT now(),
    deleted_at      timestamptz,

    CONSTRAINT fk_customer_tags_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT uq_customer_tags_tenant_code
        UNIQUE (tenant_id, tag_code)
);

CREATE INDEX IF NOT EXISTS ix_customer_tags_tenant_id
    ON crm.customer_tags (tenant_id);

CREATE INDEX IF NOT EXISTS ix_customer_tags_is_active
    ON crm.customer_tags (is_active);

DROP TRIGGER IF EXISTS trg_customer_tags_updated_at ON crm.customer_tags;
CREATE TRIGGER trg_customer_tags_updated_at
BEFORE UPDATE ON crm.customer_tags
FOR EACH ROW EXECUTE FUNCTION crm.set_updated_at();

CREATE TABLE IF NOT EXISTS crm.customer_tag_assignments (
    customer_id     uuid NOT NULL,
    tag_id          uuid NOT NULL,
    assigned_by_user_id uuid,
    assigned_at     timestamptz NOT NULL DEFAULT now(),

    PRIMARY KEY (customer_id, tag_id),

    CONSTRAINT fk_customer_tag_assignments_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_customer_tag_assignments_tag
        FOREIGN KEY (tag_id) REFERENCES crm.customer_tags (tag_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_customer_tag_assignments_assigned_by
        FOREIGN KEY (assigned_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_customer_tag_assignments_tag_id
    ON crm.customer_tag_assignments (tag_id);

-- =========================================================
-- SEGMENTS
-- =========================================================

CREATE TABLE IF NOT EXISTS crm.customer_segments (
    segment_id      uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id       uuid NOT NULL,
    segment_code    varchar(50)  NOT NULL,
    segment_name    varchar(100) NOT NULL,
    segment_type    crm.segment_type NOT NULL DEFAULT 'custom',
    description     text,
    is_active       boolean NOT NULL DEFAULT true,
    created_at      timestamptz NOT NULL DEFAULT now(),
    updated_at      timestamptz NOT NULL DEFAULT now(),
    deleted_at      timestamptz,

    CONSTRAINT fk_customer_segments_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT uq_customer_segments_tenant_code
        UNIQUE (tenant_id, segment_code)
);

CREATE INDEX IF NOT EXISTS ix_customer_segments_tenant_id
    ON crm.customer_segments (tenant_id);

CREATE INDEX IF NOT EXISTS ix_customer_segments_segment_type
    ON crm.customer_segments (segment_type);

DROP TRIGGER IF EXISTS trg_customer_segments_updated_at ON crm.customer_segments;
CREATE TRIGGER trg_customer_segments_updated_at
BEFORE UPDATE ON crm.customer_segments
FOR EACH ROW EXECUTE FUNCTION crm.set_updated_at();

CREATE TABLE IF NOT EXISTS crm.customer_segment_assignments (
    customer_id     uuid NOT NULL,
    segment_id      uuid NOT NULL,
    assigned_by_user_id uuid,
    assigned_at     timestamptz NOT NULL DEFAULT now(),

    PRIMARY KEY (customer_id, segment_id),

    CONSTRAINT fk_customer_segment_assignments_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_customer_segment_assignments_segment
        FOREIGN KEY (segment_id) REFERENCES crm.customer_segments (segment_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_customer_segment_assignments_assigned_by
        FOREIGN KEY (assigned_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_customer_segment_assignments_segment_id
    ON crm.customer_segment_assignments (segment_id);

-- =========================================================
-- CUSTOMER NOTES
-- Internal sales/compliance/account notes
-- =========================================================

CREATE TABLE IF NOT EXISTS crm.customer_notes (
    note_id            uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id          uuid NOT NULL,
    customer_id        uuid NOT NULL,

    title              varchar(200),
    body               text NOT NULL,
    visibility         crm.note_visibility NOT NULL DEFAULT 'private',

    pinned             boolean NOT NULL DEFAULT false,
    is_compliance_note  boolean NOT NULL DEFAULT false,

    created_by_user_id  uuid NOT NULL,
    updated_by_user_id  uuid,
    created_at         timestamptz NOT NULL DEFAULT now(),
    updated_at         timestamptz NOT NULL DEFAULT now(),
    deleted_at         timestamptz,

    CONSTRAINT fk_customer_notes_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_customer_notes_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_customer_notes_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_customer_notes_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_customer_notes_tenant_id
    ON crm.customer_notes (tenant_id);

CREATE INDEX IF NOT EXISTS ix_customer_notes_customer_id
    ON crm.customer_notes (customer_id);

CREATE INDEX IF NOT EXISTS ix_customer_notes_created_at
    ON crm.customer_notes (created_at);

CREATE INDEX IF NOT EXISTS ix_customer_notes_visibility
    ON crm.customer_notes (visibility);

DROP TRIGGER IF EXISTS trg_customer_notes_updated_at ON crm.customer_notes;
CREATE TRIGGER trg_customer_notes_updated_at
BEFORE UPDATE ON crm.customer_notes
FOR EACH ROW EXECUTE FUNCTION crm.set_updated_at();

-- =========================================================
-- CUSTOMER DOCUMENTS
-- Contracts, NDAs, supporting files, IDs, etc.
-- =========================================================

CREATE TABLE IF NOT EXISTS crm.customer_documents (
    document_id          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id            uuid NOT NULL,
    customer_id          uuid NOT NULL,

    document_category    varchar(100) NOT NULL,
    document_type        varchar(100) NOT NULL,
    file_name            varchar(255) NOT NULL,
    file_extension       varchar(20),
    mime_type            varchar(100),
    storage_provider     varchar(50) NOT NULL DEFAULT 'object_storage',
    storage_key          text NOT NULL,
    file_size_bytes      bigint,
    checksum_sha256      char(64),

    status               crm.document_status NOT NULL DEFAULT 'uploaded',
    visibility_level     varchar(50) NOT NULL DEFAULT 'restricted',
    is_sensitive         boolean NOT NULL DEFAULT false,

    issue_date           date,
    expiration_date      date,

    uploaded_by_user_id  uuid,
    uploaded_at          timestamptz NOT NULL DEFAULT now(),
    verified_by_user_id  uuid,
    verified_at          timestamptz,
    verification_notes   text,

    metadata             jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at           timestamptz NOT NULL DEFAULT now(),
    updated_at           timestamptz NOT NULL DEFAULT now(),
    deleted_at           timestamptz,

    CONSTRAINT fk_customer_documents_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_customer_documents_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_customer_documents_uploaded_by
        FOREIGN KEY (uploaded_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_customer_documents_verified_by
        FOREIGN KEY (verified_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_customer_documents_tenant_id
    ON crm.customer_documents (tenant_id);

CREATE INDEX IF NOT EXISTS ix_customer_documents_customer_id
    ON crm.customer_documents (customer_id);

CREATE INDEX IF NOT EXISTS ix_customer_documents_category
    ON crm.customer_documents (document_category);

CREATE INDEX IF NOT EXISTS ix_customer_documents_type
    ON crm.customer_documents (document_type);

CREATE INDEX IF NOT EXISTS ix_customer_documents_status
    ON crm.customer_documents (status);

CREATE INDEX IF NOT EXISTS ix_customer_documents_expiration_date
    ON crm.customer_documents (expiration_date);

DROP TRIGGER IF EXISTS trg_customer_documents_updated_at ON crm.customer_documents;
CREATE TRIGGER trg_customer_documents_updated_at
BEFORE UPDATE ON crm.customer_documents
FOR EACH ROW EXECUTE FUNCTION crm.set_updated_at();

-- =========================================================
-- PRICING PLANS
-- B2B pricing profiles / commercial terms
-- =========================================================

CREATE TABLE IF NOT EXISTS crm.pricing_plans (
    pricing_plan_id      uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id            uuid NOT NULL,
    customer_id          uuid NOT NULL,

    plan_code            varchar(50)  NOT NULL,
    plan_name            varchar(200) NOT NULL,
    pricing_model        crm.pricing_model NOT NULL DEFAULT 'custom',

    currency_code        char(3) NOT NULL DEFAULT 'USD',
    effective_from       date NOT NULL,
    effective_to         date,
    status               crm.contract_status NOT NULL DEFAULT 'draft',

    base_rate            numeric(18,2),
    minimum_monthly_fee  numeric(18,2),
    volume_threshold     integer,
    notes                text,
    metadata             jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id   uuid NOT NULL,
    created_at           timestamptz NOT NULL DEFAULT now(),
    updated_at           timestamptz NOT NULL DEFAULT now(),
    deleted_at           timestamptz,

    CONSTRAINT fk_pricing_plans_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_pricing_plans_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_pricing_plans_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT uq_pricing_plans_customer_code
        UNIQUE (customer_id, plan_code),

    CONSTRAINT chk_pricing_plans_base_rate
        CHECK (base_rate IS NULL OR base_rate >= 0),

    CONSTRAINT chk_pricing_plans_min_monthly_fee
        CHECK (minimum_monthly_fee IS NULL OR minimum_monthly_fee >= 0),

    CONSTRAINT chk_pricing_plans_volume_threshold
        CHECK (volume_threshold IS NULL OR volume_threshold >= 0)
);

CREATE INDEX IF NOT EXISTS ix_pricing_plans_tenant_id
    ON crm.pricing_plans (tenant_id);

CREATE INDEX IF NOT EXISTS ix_pricing_plans_customer_id
    ON crm.pricing_plans (customer_id);

CREATE INDEX IF NOT EXISTS ix_pricing_plans_status
    ON crm.pricing_plans (status);

CREATE INDEX IF NOT EXISTS ix_pricing_plans_pricing_model
    ON crm.pricing_plans (pricing_model);

DROP TRIGGER IF EXISTS trg_pricing_plans_updated_at ON crm.pricing_plans;
CREATE TRIGGER trg_pricing_plans_updated_at
BEFORE UPDATE ON crm.pricing_plans
FOR EACH ROW EXECUTE FUNCTION crm.set_updated_at();

-- =========================================================
-- PRICING RULES
-- Fixed / volume-based / tiered commercial rules
-- =========================================================

CREATE TABLE IF NOT EXISTS crm.pricing_rules (
    pricing_rule_id      uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id            uuid NOT NULL,
    pricing_plan_id      uuid NOT NULL,

    rule_code            varchar(50)  NOT NULL,
    rule_name            varchar(200) NOT NULL,
    rule_type            crm.pricing_rule_type NOT NULL DEFAULT 'base_rate',

    service_type         varchar(100),
    min_quantity         integer,
    max_quantity         integer,
    flat_fee             numeric(18,2),
    unit_price           numeric(18,2),
    discount_percent     numeric(5,2),
    surcharge_amount     numeric(18,2),

    effective_from       date NOT NULL,
    effective_to         date,
    is_active            boolean NOT NULL DEFAULT true,
    notes                text,
    metadata             jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at           timestamptz NOT NULL DEFAULT now(),
    updated_at           timestamptz NOT NULL DEFAULT now(),
    deleted_at           timestamptz,

    CONSTRAINT fk_pricing_rules_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_pricing_rules_plan
        FOREIGN KEY (pricing_plan_id) REFERENCES crm.pricing_plans (pricing_plan_id)
        ON DELETE CASCADE,

    CONSTRAINT uq_pricing_rules_plan_code
        UNIQUE (pricing_plan_id, rule_code),

    CONSTRAINT chk_pricing_rules_min_quantity
        CHECK (min_quantity IS NULL OR min_quantity >= 0),

    CONSTRAINT chk_pricing_rules_max_quantity
        CHECK (max_quantity IS NULL OR max_quantity >= 0),

    CONSTRAINT chk_pricing_rules_flat_fee
        CHECK (flat_fee IS NULL OR flat_fee >= 0),

    CONSTRAINT chk_pricing_rules_unit_price
        CHECK (unit_price IS NULL OR unit_price >= 0),

    CONSTRAINT chk_pricing_rules_discount_percent
        CHECK (discount_percent IS NULL OR (discount_percent >= 0 AND discount_percent <= 100)),

    CONSTRAINT chk_pricing_rules_surcharge_amount
        CHECK (surcharge_amount IS NULL OR surcharge_amount >= 0)
);

CREATE INDEX IF NOT EXISTS ix_pricing_rules_tenant_id
    ON crm.pricing_rules (tenant_id);

CREATE INDEX IF NOT EXISTS ix_pricing_rules_pricing_plan_id
    ON crm.pricing_rules (pricing_plan_id);

CREATE INDEX IF NOT EXISTS ix_pricing_rules_rule_type
    ON crm.pricing_rules (rule_type);

CREATE INDEX IF NOT EXISTS ix_pricing_rules_service_type
    ON crm.pricing_rules (service_type);

DROP TRIGGER IF EXISTS trg_pricing_rules_updated_at ON crm.pricing_rules;
CREATE TRIGGER trg_pricing_rules_updated_at
BEFORE UPDATE ON crm.pricing_rules
FOR EACH ROW EXECUTE FUNCTION crm.set_updated_at();

-- =========================================================
-- CONTRACTS
-- B2B commercial contracts with title companies, law firms, banks, etc.
-- =========================================================

CREATE TABLE IF NOT EXISTS crm.contracts (
    contract_id          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id            uuid NOT NULL,
    customer_id          uuid NOT NULL,
    pricing_plan_id      uuid,

    contract_number      varchar(100) NOT NULL,
    contract_name        varchar(200) NOT NULL,
    status               crm.contract_status NOT NULL DEFAULT 'draft',

    effective_date       date NOT NULL,
    expiration_date      date,
    signed_date          date,
    auto_renew           boolean NOT NULL DEFAULT false,
    renewal_notice_days  integer NOT NULL DEFAULT 30,

    sla_summary          text,
    commercial_terms     text,
    notes                text,
    metadata             jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id   uuid NOT NULL,
    updated_by_user_id   uuid,
    created_at           timestamptz NOT NULL DEFAULT now(),
    updated_at           timestamptz NOT NULL DEFAULT now(),
    deleted_at           timestamptz,

    CONSTRAINT fk_contracts_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_contracts_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_contracts_pricing_plan
        FOREIGN KEY (pricing_plan_id) REFERENCES crm.pricing_plans (pricing_plan_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_contracts_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_contracts_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_contracts_customer_number
        UNIQUE (customer_id, contract_number),

    CONSTRAINT chk_contracts_renewal_notice_days
        CHECK (renewal_notice_days >= 0),

    CONSTRAINT chk_contracts_dates
        CHECK (expiration_date IS NULL OR expiration_date > effective_date)
);

CREATE INDEX IF NOT EXISTS ix_contracts_tenant_id
    ON crm.contracts (tenant_id);

CREATE INDEX IF NOT EXISTS ix_contracts_customer_id
    ON crm.contracts (customer_id);

CREATE INDEX IF NOT EXISTS ix_contracts_status
    ON crm.contracts (status);

CREATE INDEX IF NOT EXISTS ix_contracts_effective_date
    ON crm.contracts (effective_date);

CREATE INDEX IF NOT EXISTS ix_contracts_expiration_date
    ON crm.contracts (expiration_date);

DROP TRIGGER IF EXISTS trg_contracts_updated_at ON crm.contracts;
CREATE TRIGGER trg_contracts_updated_at
BEFORE UPDATE ON crm.contracts
FOR EACH ROW EXECUTE FUNCTION crm.set_updated_at();

-- =========================================================
-- CONTRACT DOCUMENTS
-- Uploaded contract files and annexes
-- =========================================================

CREATE TABLE IF NOT EXISTS crm.contract_documents (
    contract_document_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id            uuid NOT NULL,
    contract_id          uuid NOT NULL,

    document_type        varchar(100) NOT NULL,
    file_name            varchar(255) NOT NULL,
    file_extension       varchar(20),
    mime_type            varchar(100),
    storage_provider     varchar(50) NOT NULL DEFAULT 'object_storage',
    storage_key          text NOT NULL,
    file_size_bytes      bigint,
    checksum_sha256      char(64),

    version_no           integer NOT NULL DEFAULT 1,
    uploaded_by_user_id  uuid NOT NULL,
    uploaded_at          timestamptz NOT NULL DEFAULT now(),
    notes                text,
    metadata             jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at           timestamptz NOT NULL DEFAULT now(),
    updated_at           timestamptz NOT NULL DEFAULT now(),
    deleted_at           timestamptz,

    CONSTRAINT fk_contract_documents_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_contract_documents_contract
        FOREIGN KEY (contract_id) REFERENCES crm.contracts (contract_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_contract_documents_uploaded_by
        FOREIGN KEY (uploaded_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS ix_contract_documents_tenant_id
    ON crm.contract_documents (tenant_id);

CREATE INDEX IF NOT EXISTS ix_contract_documents_contract_id
    ON crm.contract_documents (contract_id);

CREATE INDEX IF NOT EXISTS ix_contract_documents_document_type
    ON crm.contract_documents (document_type);

DROP TRIGGER IF EXISTS trg_contract_documents_updated_at ON crm.contract_documents;
CREATE TRIGGER trg_contract_documents_updated_at
BEFORE UPDATE ON crm.contract_documents
FOR EACH ROW EXECUTE FUNCTION crm.set_updated_at();

-- =========================================================
-- SLA AGREEMENTS
-- Separate SLA entity when needed
-- =========================================================

CREATE TABLE IF NOT EXISTS crm.sla_agreements (
    sla_id               uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id            uuid NOT NULL,
    contract_id          uuid NOT NULL,

    sla_code             varchar(50)  NOT NULL,
    sla_name             varchar(200) NOT NULL,
    status               crm.sla_status NOT NULL DEFAULT 'draft',

    target_turnaround_minutes integer,
    on_time_target_percent    numeric(5,2),
    penalty_terms        text,
    service_hours        text,
    response_time_minutes integer,
    escalation_rules     text,
    notes                text,
    metadata             jsonb NOT NULL DEFAULT '{}'::jsonb,

    effective_from       date NOT NULL,
    effective_to         date,
    created_by_user_id   uuid NOT NULL,
    updated_by_user_id   uuid,
    created_at           timestamptz NOT NULL DEFAULT now(),
    updated_at           timestamptz NOT NULL DEFAULT now(),
    deleted_at           timestamptz,

    CONSTRAINT fk_sla_agreements_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_sla_agreements_contract
        FOREIGN KEY (contract_id) REFERENCES crm.contracts (contract_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_sla_agreements_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_sla_agreements_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_sla_agreements_contract_code
        UNIQUE (contract_id, sla_code),

    CONSTRAINT chk_sla_agreements_turnaround
        CHECK (target_turnaround_minutes IS NULL OR target_turnaround_minutes >= 0),

    CONSTRAINT chk_sla_agreements_on_time_target
        CHECK (on_time_target_percent IS NULL OR (on_time_target_percent >= 0 AND on_time_target_percent <= 100)),

    CONSTRAINT chk_sla_agreements_response_time
        CHECK (response_time_minutes IS NULL OR response_time_minutes >= 0),

    CONSTRAINT chk_sla_agreements_dates
        CHECK (effective_to IS NULL OR effective_to > effective_from)
);

CREATE INDEX IF NOT EXISTS ix_sla_agreements_tenant_id
    ON crm.sla_agreements (tenant_id);

CREATE INDEX IF NOT EXISTS ix_sla_agreements_contract_id
    ON crm.sla_agreements (contract_id);

CREATE INDEX IF NOT EXISTS ix_sla_agreements_status
    ON crm.sla_agreements (status);

DROP TRIGGER IF EXISTS trg_sla_agreements_updated_at ON crm.sla_agreements;
CREATE TRIGGER trg_sla_agreements_updated_at
BEFORE UPDATE ON crm.sla_agreements
FOR EACH ROW EXECUTE FUNCTION crm.set_updated_at();

-- =========================================================
-- CUSTOMER STATUS HISTORY
-- Audit-friendly state transitions
-- =========================================================

CREATE TABLE IF NOT EXISTS crm.customer_status_history (
    status_history_id     uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id             uuid NOT NULL,
    customer_id           uuid NOT NULL,

    previous_status       crm.customer_status,
    new_status            crm.customer_status NOT NULL,
    reason                text,
    effective_at          timestamptz NOT NULL DEFAULT now(),

    changed_by_user_id    uuid,
    source_reference      text,
    metadata              jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at            timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_customer_status_history_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_customer_status_history_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_customer_status_history_changed_by
        FOREIGN KEY (changed_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_customer_status_history_tenant_id
    ON crm.customer_status_history (tenant_id);

CREATE INDEX IF NOT EXISTS ix_customer_status_history_customer_id
    ON crm.customer_status_history (customer_id);

CREATE INDEX IF NOT EXISTS ix_customer_status_history_effective_at
    ON crm.customer_status_history (effective_at);

-- =========================================================
-- OPTIONAL VIEWS
-- =========================================================

CREATE OR REPLACE VIEW crm.v_customer_overview AS
SELECT
    c.customer_id,
    c.tenant_id,
    c.customer_code,
    c.customer_type,
    c.status,
    c.display_name,
    c.legal_name,
    c.industry,
    c.primary_email,
    c.primary_phone,
    c.billing_email,
    c.billing_phone,
    c.total_jobs,
    c.total_revenue,
    c.avg_turnaround_minutes,
    c.account_manager_user_id,
    c.branch_id,
    c.region_id,
    c.created_at,
    c.updated_at
FROM crm.customers c
WHERE c.deleted_at IS NULL;

CREATE OR REPLACE VIEW crm.v_customer_primary_contact AS
SELECT
    c.customer_id,
    c.tenant_id,
    c.display_name AS customer_name,
    ct.contact_id,
    ct.full_name,
    ct.role,
    ct.email,
    ct.phone,
    ct.mobile_phone
FROM crm.customers c
LEFT JOIN crm.customer_contacts ct
    ON ct.customer_id = c.customer_id
   AND ct.is_primary = true
   AND ct.deleted_at IS NULL
WHERE c.deleted_at IS NULL;

-- =========================================================
-- COMMENTS
-- =========================================================

COMMENT ON SCHEMA crm IS 'CRM schema for customer/account, contacts, pricing, contracts, SLA, notes, and documents.';
COMMENT ON TABLE crm.customers IS 'Core customer/account entity for B2C and B2B.';
COMMENT ON TABLE crm.customer_contacts IS 'Multiple contacts per customer/account.';
COMMENT ON TABLE crm.customer_contact_preferences IS 'Notification and communication preferences for contacts.';
COMMENT ON TABLE crm.customer_tags IS 'Tag catalog for segmentation.';
COMMENT ON TABLE crm.customer_tag_assignments IS 'Customer-to-tag mapping.';
COMMENT ON TABLE crm.customer_segments IS 'Segmentation catalog.';
COMMENT ON TABLE crm.customer_segment_assignments IS 'Customer-to-segment mapping.';
COMMENT ON TABLE crm.customer_notes IS 'Internal notes and compliance notes.';
COMMENT ON TABLE crm.customer_documents IS 'Customer-related documents and files.';
COMMENT ON TABLE crm.pricing_plans IS 'Commercial pricing plan linked to a customer.';
COMMENT ON TABLE crm.pricing_rules IS 'Detailed pricing rules and tiers.';
COMMENT ON TABLE crm.contracts IS 'Commercial contracts with B2B customers.';
COMMENT ON TABLE crm.contract_documents IS 'Uploaded contract files and annexes.';
COMMENT ON TABLE crm.sla_agreements IS 'Service-level agreements linked to contracts.';
COMMENT ON TABLE crm.customer_status_history IS 'Status transition history for audit and compliance.';


-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================

-- PostgreSQL 15+
-- Operations schema for job requests, jobs, dispatch, scheduling, availability,
-- reminders, and operational timeline management.

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

CREATE SCHEMA IF NOT EXISTS operations;

-- =========================================================
-- ENUM TYPES
-- =========================================================

DO $$
BEGIN
    IF to_regtype('operations.service_mode') IS NULL THEN
        CREATE TYPE operations.service_mode AS ENUM (
            'onsite',
            'mobile',
            'ron',
            'hybrid'
        );
    END IF;

    IF to_regtype('operations.job_priority') IS NULL THEN
        CREATE TYPE operations.job_priority AS ENUM (
            'low',
            'normal',
            'high',
            'urgent',
            'rush'
        );
    END IF;

    IF to_regtype('operations.job_request_status') IS NULL THEN
        CREATE TYPE operations.job_request_status AS ENUM (
            'new',
            'triaged',
            'quoted',
            'scheduled',
            'converted',
            'rejected',
            'cancelled',
            'closed'
        );
    END IF;

    IF to_regtype('operations.job_status') IS NULL THEN
        CREATE TYPE operations.job_status AS ENUM (
            'draft',
            'scheduled',
            'confirmed',
            'in_progress',
            'completed',
            'locked',
            'cancelled',
            'failed',
            'archived'
        );
    END IF;

    IF to_regtype('operations.assignment_status') IS NULL THEN
        CREATE TYPE operations.assignment_status AS ENUM (
            'proposed',
            'assigned',
            'accepted',
            'declined',
            'released',
            'replaced',
            'completed'
        );
    END IF;

    IF to_regtype('operations.assignment_role') IS NULL THEN
        CREATE TYPE operations.assignment_role AS ENUM (
            'primary',
            'backup',
            'observer',
            'substitute'
        );
    END IF;

    IF to_regtype('operations.schedule_block_type') IS NULL THEN
        CREATE TYPE operations.schedule_block_type AS ENUM (
            'job',
            'shift',
            'break',
            'travel',
            'hold',
            'meeting',
            'blackout'
        );
    END IF;

    IF to_regtype('operations.availability_rule_type') IS NULL THEN
        CREATE TYPE operations.availability_rule_type AS ENUM (
            'working_hours',
            'preferred_hours',
            'on_call',
            'blackout',
            'unavailable'
        );
    END IF;

    IF to_regtype('operations.shift_request_status') IS NULL THEN
        CREATE TYPE operations.shift_request_status AS ENUM (
            'pending',
            'approved',
            'rejected',
            'cancelled',
            'completed'
        );
    END IF;

    IF to_regtype('operations.dispatch_run_status') IS NULL THEN
        CREATE TYPE operations.dispatch_run_status AS ENUM (
            'queued',
            'running',
            'completed',
            'failed',
            'cancelled'
        );
    END IF;

    IF to_regtype('operations.dispatch_candidate_status') IS NULL THEN
        CREATE TYPE operations.dispatch_candidate_status AS ENUM (
            'eligible',
            'preferred',
            'warning',
            'ineligible',
            'rejected'
        );
    END IF;

    IF to_regtype('operations.dispatch_rule_type') IS NULL THEN
        CREATE TYPE operations.dispatch_rule_type AS ENUM (
            'distance',
            'service_type',
            'compliance',
            'workload',
            'priority',
            'state_restriction',
            'manual_override'
        );
    END IF;

    IF to_regtype('operations.timeline_event_type') IS NULL THEN
        CREATE TYPE operations.timeline_event_type AS ENUM (
            'status_change',
            'assignment_change',
            'note',
            'reminder',
            'reschedule',
            'location_change',
            'compliance_flag',
            'escalation',
            'attachment_added'
        );
    END IF;

    IF to_regtype('operations.reminder_channel') IS NULL THEN
        CREATE TYPE operations.reminder_channel AS ENUM (
            'email',
            'sms',
            'in_app'
        );
    END IF;

    IF to_regtype('operations.reminder_status') IS NULL THEN
        CREATE TYPE operations.reminder_status AS ENUM (
            'pending',
            'queued',
            'sent',
            'delivered',
            'failed',
            'cancelled'
        );
    END IF;

    IF to_regtype('operations.reminder_recipient_type') IS NULL THEN
        CREATE TYPE operations.reminder_recipient_type AS ENUM (
            'contact',
            'user',
            'notary'
        );
    END IF;

    IF to_regtype('operations.notification_scope') IS NULL THEN
        CREATE TYPE operations.notification_scope AS ENUM (
            'internal',
            'customer',
            'notary',
            'mixed'
        );
    END IF;
END $$;

-- =========================================================
-- COMMON UPDATED_AT TRIGGER
-- =========================================================

CREATE OR REPLACE FUNCTION operations.set_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at = now();
    RETURN NEW;
END;
$$;

-- =========================================================
-- SERVICE TYPES
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.service_types (
    service_type_id          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    service_code             varchar(50)  NOT NULL,
    service_name             varchar(200) NOT NULL,
    category                 varchar(100),
    description              text,

    default_mode             operations.service_mode NOT NULL DEFAULT 'onsite',
    default_duration_minutes integer,
    requires_identity_verification boolean NOT NULL DEFAULT true,
    requires_journal_entry   boolean NOT NULL DEFAULT true,
    requires_seal            boolean NOT NULL DEFAULT true,
    requires_signature       boolean NOT NULL DEFAULT true,

    is_active                boolean NOT NULL DEFAULT true,
    settings                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_service_types_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT uq_service_types_tenant_code
        UNIQUE (tenant_id, service_code),

    CONSTRAINT chk_service_types_default_duration
        CHECK (default_duration_minutes IS NULL OR default_duration_minutes >= 0)
);

CREATE INDEX IF NOT EXISTS ix_service_types_tenant_id
    ON operations.service_types (tenant_id);

CREATE INDEX IF NOT EXISTS ix_service_types_is_active
    ON operations.service_types (is_active);

DROP TRIGGER IF EXISTS trg_service_types_updated_at ON operations.service_types;
CREATE TRIGGER trg_service_types_updated_at
BEFORE UPDATE ON operations.service_types
FOR EACH ROW EXECUTE FUNCTION operations.set_updated_at();

-- =========================================================
-- JOB REQUESTS
-- Intake / orders / requests before conversion to a job
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.job_requests (
    job_request_id            uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    request_code              varchar(50)  NOT NULL,

    customer_id               uuid NOT NULL,
    customer_contact_id       uuid,
    created_by_user_id        uuid,
    source_channel            varchar(50) NOT NULL DEFAULT 'internal',

    service_type_id           uuid NOT NULL,
    service_mode              operations.service_mode NOT NULL DEFAULT 'onsite',
    priority                  operations.job_priority NOT NULL DEFAULT 'normal',
    status                    operations.job_request_status NOT NULL DEFAULT 'new',

    requested_start_at        timestamptz,
    requested_end_at          timestamptz,
    preferred_date            date,
    timezone                  varchar(64) NOT NULL DEFAULT 'UTC',

    estimated_duration_minutes integer,
    rush_flag                 boolean NOT NULL DEFAULT false,
    special_instructions      text,

    branch_id                 uuid,
    region_id                 uuid,

    location_name             varchar(200),
    address_line1             varchar(200),
    address_line2             varchar(200),
    city                      varchar(100),
    state_code                char(2),
    postal_code               varchar(20),
    country_code              char(2),
    latitude                  numeric(10,7),
    longitude                 numeric(10,7),
    virtual_meeting_url       text,

    sla_due_at                timestamptz,
    quoted_amount             numeric(18,2),
    quote_currency_code       char(3) NOT NULL DEFAULT 'USD',

    assigned_dispatcher_user_id uuid,
    triaged_at                timestamptz,
    scheduled_at              timestamptz,
    cancelled_at              timestamptz,
    cancelled_by_user_id      uuid,
    cancel_reason             text,

    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_job_requests_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_job_requests_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_job_requests_customer_contact
        FOREIGN KEY (customer_contact_id) REFERENCES crm.customer_contacts (contact_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_job_requests_created_by_user
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_job_requests_service_type
        FOREIGN KEY (service_type_id) REFERENCES operations.service_types (service_type_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_job_requests_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_job_requests_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_job_requests_assigned_dispatcher
        FOREIGN KEY (assigned_dispatcher_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_job_requests_cancelled_by
        FOREIGN KEY (cancelled_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_job_requests_tenant_code
        UNIQUE (tenant_id, request_code),

    CONSTRAINT chk_job_requests_estimated_duration
        CHECK (estimated_duration_minutes IS NULL OR estimated_duration_minutes >= 0),

    CONSTRAINT chk_job_requests_quoted_amount
        CHECK (quoted_amount IS NULL OR quoted_amount >= 0)
);

CREATE INDEX IF NOT EXISTS ix_job_requests_tenant_id
    ON operations.job_requests (tenant_id);

CREATE INDEX IF NOT EXISTS ix_job_requests_customer_id
    ON operations.job_requests (customer_id);

CREATE INDEX IF NOT EXISTS ix_job_requests_service_type_id
    ON operations.job_requests (service_type_id);

CREATE INDEX IF NOT EXISTS ix_job_requests_status
    ON operations.job_requests (status);

CREATE INDEX IF NOT EXISTS ix_job_requests_priority
    ON operations.job_requests (priority);

CREATE INDEX IF NOT EXISTS ix_job_requests_requested_start_at
    ON operations.job_requests (requested_start_at);

CREATE INDEX IF NOT EXISTS ix_job_requests_sla_due_at
    ON operations.job_requests (sla_due_at);

DROP TRIGGER IF EXISTS trg_job_requests_updated_at ON operations.job_requests;
CREATE TRIGGER trg_job_requests_updated_at
BEFORE UPDATE ON operations.job_requests
FOR EACH ROW EXECUTE FUNCTION operations.set_updated_at();

-- =========================================================
-- JOBS
-- Confirmed operational records after intake/dispatch
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.jobs (
    job_id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    job_code                  varchar(50)  NOT NULL,

    job_request_id            uuid,
    customer_id               uuid NOT NULL,
    customer_contact_id       uuid,

    service_type_id           uuid NOT NULL,
    service_mode              operations.service_mode NOT NULL DEFAULT 'onsite',
    priority                  operations.job_priority NOT NULL DEFAULT 'normal',
    status                    operations.job_status NOT NULL DEFAULT 'draft',

    branch_id                 uuid,
    region_id                 uuid,

    requested_start_at        timestamptz,
    requested_end_at          timestamptz,

    scheduled_start_at        timestamptz,
    scheduled_end_at          timestamptz,
    actual_start_at           timestamptz,
    actual_end_at             timestamptz,

    timezone                  varchar(64) NOT NULL DEFAULT 'UTC',
    estimated_duration_minutes integer,
    actual_duration_minutes   integer,

    location_name             varchar(200),
    address_line1             varchar(200),
    address_line2             varchar(200),
    city                      varchar(100),
    state_code                char(2),
    postal_code               varchar(20),
    country_code              char(2),
    latitude                  numeric(10,7),
    longitude                 numeric(10,7),
    virtual_meeting_url       text,

    sla_due_at                timestamptz,
    rush_flag                 boolean NOT NULL DEFAULT false,
    lock_required             boolean NOT NULL DEFAULT false,

    locked_at                 timestamptz,
    locked_by_user_id         uuid,
    locked_reason             text,

    cancelled_at              timestamptz,
    cancelled_by_user_id      uuid,
    cancel_reason             text,

    completed_at              timestamptz,
    failed_at                 timestamptz,
    failure_reason            text,

    notes                     text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id        uuid,
    updated_by_user_id        uuid,
    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_jobs_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_jobs_request
        FOREIGN KEY (job_request_id) REFERENCES operations.job_requests (job_request_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_jobs_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_jobs_customer_contact
        FOREIGN KEY (customer_contact_id) REFERENCES crm.customer_contacts (contact_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_jobs_service_type
        FOREIGN KEY (service_type_id) REFERENCES operations.service_types (service_type_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_jobs_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_jobs_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_jobs_locked_by_user
        FOREIGN KEY (locked_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_jobs_cancelled_by_user
        FOREIGN KEY (cancelled_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_jobs_created_by_user
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_jobs_updated_by_user
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_jobs_tenant_code
        UNIQUE (tenant_id, job_code),

    CONSTRAINT chk_jobs_estimated_duration
        CHECK (estimated_duration_minutes IS NULL OR estimated_duration_minutes >= 0),

    CONSTRAINT chk_jobs_actual_duration
        CHECK (actual_duration_minutes IS NULL OR actual_duration_minutes >= 0)
);

CREATE INDEX IF NOT EXISTS ix_jobs_tenant_id
    ON operations.jobs (tenant_id);

CREATE INDEX IF NOT EXISTS ix_jobs_job_request_id
    ON operations.jobs (job_request_id);

CREATE INDEX IF NOT EXISTS ix_jobs_customer_id
    ON operations.jobs (customer_id);

CREATE INDEX IF NOT EXISTS ix_jobs_service_type_id
    ON operations.jobs (service_type_id);

CREATE INDEX IF NOT EXISTS ix_jobs_status
    ON operations.jobs (status);

CREATE INDEX IF NOT EXISTS ix_jobs_priority
    ON operations.jobs (priority);

CREATE INDEX IF NOT EXISTS ix_jobs_scheduled_start_at
    ON operations.jobs (scheduled_start_at);

CREATE INDEX IF NOT EXISTS ix_jobs_sla_due_at
    ON operations.jobs (sla_due_at);

DROP TRIGGER IF EXISTS trg_jobs_updated_at ON operations.jobs;
CREATE TRIGGER trg_jobs_updated_at
BEFORE UPDATE ON operations.jobs
FOR EACH ROW EXECUTE FUNCTION operations.set_updated_at();

-- =========================================================
-- JOB ASSIGNMENTS
-- Primary / backup / substitute assignment history
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.job_assignments (
    job_assignment_id         uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    job_id                    uuid NOT NULL,
    notary_id                 uuid NOT NULL,

    assignment_role           operations.assignment_role NOT NULL DEFAULT 'primary',
    status                    operations.assignment_status NOT NULL DEFAULT 'proposed',
    is_primary                boolean NOT NULL DEFAULT false,

    assigned_by_user_id       uuid,
    released_by_user_id       uuid,

    proposed_at               timestamptz NOT NULL DEFAULT now(),
    assigned_at               timestamptz,
    accepted_at               timestamptz,
    declined_at               timestamptz,
    released_at               timestamptz,
    completed_at              timestamptz,

    distance_km               numeric(10,2),
    travel_minutes            integer,
    compliance_snapshot       jsonb NOT NULL DEFAULT '{}'::jsonb,
    performance_snapshot      jsonb NOT NULL DEFAULT '{}'::jsonb,
    notes                     text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_job_assignments_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_job_assignments_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_job_assignments_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_job_assignments_assigned_by
        FOREIGN KEY (assigned_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_job_assignments_released_by
        FOREIGN KEY (released_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT chk_job_assignments_distance
        CHECK (distance_km IS NULL OR distance_km >= 0),

    CONSTRAINT chk_job_assignments_travel_minutes
        CHECK (travel_minutes IS NULL OR travel_minutes >= 0)
);

CREATE INDEX IF NOT EXISTS ix_job_assignments_tenant_id
    ON operations.job_assignments (tenant_id);

CREATE INDEX IF NOT EXISTS ix_job_assignments_job_id
    ON operations.job_assignments (job_id);

CREATE INDEX IF NOT EXISTS ix_job_assignments_notary_id
    ON operations.job_assignments (notary_id);

CREATE INDEX IF NOT EXISTS ix_job_assignments_status
    ON operations.job_assignments (status);

CREATE INDEX IF NOT EXISTS ix_job_assignments_assignment_role
    ON operations.job_assignments (assignment_role);

CREATE INDEX IF NOT EXISTS ix_job_assignments_is_primary
    ON operations.job_assignments (is_primary);

DROP TRIGGER IF EXISTS trg_job_assignments_updated_at ON operations.job_assignments;
CREATE TRIGGER trg_job_assignments_updated_at
BEFORE UPDATE ON operations.job_assignments
FOR EACH ROW EXECUTE FUNCTION operations.set_updated_at();

-- Only one primary assignment per job
CREATE UNIQUE INDEX IF NOT EXISTS ux_job_assignments_one_primary
    ON operations.job_assignments (job_id)
    WHERE is_primary = true AND deleted_at IS NULL;

-- =========================================================
-- JOB STATUS HISTORY
-- Immutable audit-oriented status transitions
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.job_status_history (
    job_status_history_id     uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    job_id                    uuid NOT NULL,

    previous_status           operations.job_status,
    new_status                operations.job_status NOT NULL,
    reason                    text,
    source_reference          text,

    effective_at              timestamptz NOT NULL DEFAULT now(),
    changed_by_user_id        uuid,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_job_status_history_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_job_status_history_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_job_status_history_changed_by
        FOREIGN KEY (changed_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_job_status_history_tenant_id
    ON operations.job_status_history (tenant_id);

CREATE INDEX IF NOT EXISTS ix_job_status_history_job_id
    ON operations.job_status_history (job_id);

CREATE INDEX IF NOT EXISTS ix_job_status_history_effective_at
    ON operations.job_status_history (effective_at);

CREATE INDEX IF NOT EXISTS ix_job_status_history_new_status
    ON operations.job_status_history (new_status);

-- =========================================================
-- JOB TIMELINE EVENTS
-- Flexible timeline for detail screen and operational history
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.job_timeline_events (
    job_timeline_event_id     uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    job_id                    uuid NOT NULL,

    event_type                operations.timeline_event_type NOT NULL,
    event_title               varchar(200),
    event_body                text,

    old_status                operations.job_status,
    new_status                operations.job_status,

    related_assignment_id     uuid,
    related_user_id           uuid,
    related_notary_id         uuid,

    occurred_at               timestamptz NOT NULL DEFAULT now(),
    occurred_by_user_id       uuid,
    source_reference          text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_job_timeline_events_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_job_timeline_events_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_job_timeline_events_assignment
        FOREIGN KEY (related_assignment_id) REFERENCES operations.job_assignments (job_assignment_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_job_timeline_events_related_user
        FOREIGN KEY (related_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_job_timeline_events_related_notary
        FOREIGN KEY (related_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_job_timeline_events_occurred_by
        FOREIGN KEY (occurred_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_job_timeline_events_tenant_id
    ON operations.job_timeline_events (tenant_id);

CREATE INDEX IF NOT EXISTS ix_job_timeline_events_job_id
    ON operations.job_timeline_events (job_id);

CREATE INDEX IF NOT EXISTS ix_job_timeline_events_event_type
    ON operations.job_timeline_events (event_type);

CREATE INDEX IF NOT EXISTS ix_job_timeline_events_occurred_at
    ON operations.job_timeline_events (occurred_at);

-- =========================================================
-- NOTARY AVAILABILITY RULES
-- Weekly working hours / blackout / preferred hours
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.notary_availability_rules (
    availability_rule_id      uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    notary_id                 uuid NOT NULL,

    rule_code                 varchar(50) NOT NULL,
    rule_name                 varchar(200) NOT NULL,
    rule_type                 operations.availability_rule_type NOT NULL DEFAULT 'working_hours',

    day_of_week               smallint NOT NULL, -- 0 = Sunday ... 6 = Saturday
    start_time                time,
    end_time                  time,

    effective_from            date,
    effective_to              date,
    is_active                 boolean NOT NULL DEFAULT true,

    branch_id                 uuid,
    region_id                 uuid,
    notes                     text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_availability_rules_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_availability_rules_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_availability_rules_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_availability_rules_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_availability_rules_notary_code
        UNIQUE (notary_id, rule_code),

    CONSTRAINT chk_availability_rules_day_of_week
        CHECK (day_of_week >= 0 AND day_of_week <= 6)
);

CREATE INDEX IF NOT EXISTS ix_notary_availability_rules_tenant_id
    ON operations.notary_availability_rules (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notary_availability_rules_notary_id
    ON operations.notary_availability_rules (notary_id);

CREATE INDEX IF NOT EXISTS ix_notary_availability_rules_rule_type
    ON operations.notary_availability_rules (rule_type);

CREATE INDEX IF NOT EXISTS ix_notary_availability_rules_is_active
    ON operations.notary_availability_rules (is_active);

DROP TRIGGER IF EXISTS trg_notary_availability_rules_updated_at ON operations.notary_availability_rules;
CREATE TRIGGER trg_notary_availability_rules_updated_at
BEFORE UPDATE ON operations.notary_availability_rules
FOR EACH ROW EXECUTE FUNCTION operations.set_updated_at();

-- =========================================================
-- SHIFT / TIME-OFF REQUESTS
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.notary_shift_requests (
    shift_request_id          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    notary_id                 uuid NOT NULL,

    request_code              varchar(50) NOT NULL,
    request_type              operations.availability_rule_type NOT NULL DEFAULT 'blackout',
    status                    operations.shift_request_status NOT NULL DEFAULT 'pending',

    start_at                  timestamptz NOT NULL,
    end_at                    timestamptz NOT NULL,
    reason                    text,
    branch_id                 uuid,
    region_id                 uuid,

    requested_by_user_id      uuid,
    approved_by_user_id       uuid,
    approved_at               timestamptz,
    rejected_at               timestamptz,

    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_shift_requests_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_shift_requests_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_shift_requests_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_shift_requests_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_shift_requests_requested_by
        FOREIGN KEY (requested_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_shift_requests_approved_by
        FOREIGN KEY (approved_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_shift_requests_notary_code
        UNIQUE (notary_id, request_code),

    CONSTRAINT chk_shift_requests_time_range
        CHECK (end_at > start_at)
);

CREATE INDEX IF NOT EXISTS ix_notary_shift_requests_tenant_id
    ON operations.notary_shift_requests (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notary_shift_requests_notary_id
    ON operations.notary_shift_requests (notary_id);

CREATE INDEX IF NOT EXISTS ix_notary_shift_requests_status
    ON operations.notary_shift_requests (status);

CREATE INDEX IF NOT EXISTS ix_notary_shift_requests_start_at
    ON operations.notary_shift_requests (start_at);

DROP TRIGGER IF EXISTS trg_notary_shift_requests_updated_at ON operations.notary_shift_requests;
CREATE TRIGGER trg_notary_shift_requests_updated_at
BEFORE UPDATE ON operations.notary_shift_requests
FOR EACH ROW EXECUTE FUNCTION operations.set_updated_at();

-- =========================================================
-- SCHEDULE BLOCKS
-- Master calendar entries for jobs, shifts, breaks, holds, travel
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.schedule_blocks (
    schedule_block_id         uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,

    block_code                varchar(50) NOT NULL,
    block_type                operations.schedule_block_type NOT NULL DEFAULT 'job',
    title                     varchar(200) NOT NULL,
    description               text,

    start_at                  timestamptz NOT NULL,
    end_at                    timestamptz NOT NULL,
    timezone                  varchar(64) NOT NULL DEFAULT 'UTC',

    job_id                    uuid,
    job_assignment_id         uuid,
    notary_id                 uuid,
    branch_id                 uuid,
    region_id                 uuid,

    location_name             varchar(200),
    address_line1             varchar(200),
    address_line2             varchar(200),
    city                      varchar(100),
    state_code                char(2),
    postal_code               varchar(20),
    country_code              char(2),
    latitude                  numeric(10,7),
    longitude                 numeric(10,7),

    is_all_day                boolean NOT NULL DEFAULT false,
    is_conflict               boolean NOT NULL DEFAULT false,
    conflict_reason           text,

    source_reference          text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id        uuid,
    updated_by_user_id        uuid,
    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_schedule_blocks_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_schedule_blocks_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_schedule_blocks_job_assignment
        FOREIGN KEY (job_assignment_id) REFERENCES operations.job_assignments (job_assignment_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_schedule_blocks_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_schedule_blocks_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_schedule_blocks_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_schedule_blocks_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_schedule_blocks_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_schedule_blocks_tenant_code
        UNIQUE (tenant_id, block_code),

    CONSTRAINT chk_schedule_blocks_time_range
        CHECK (end_at > start_at)
);

CREATE INDEX IF NOT EXISTS ix_schedule_blocks_tenant_id
    ON operations.schedule_blocks (tenant_id);

CREATE INDEX IF NOT EXISTS ix_schedule_blocks_job_id
    ON operations.schedule_blocks (job_id);

CREATE INDEX IF NOT EXISTS ix_schedule_blocks_job_assignment_id
    ON operations.schedule_blocks (job_assignment_id);

CREATE INDEX IF NOT EXISTS ix_schedule_blocks_notary_id
    ON operations.schedule_blocks (notary_id);

CREATE INDEX IF NOT EXISTS ix_schedule_blocks_block_type
    ON operations.schedule_blocks (block_type);

CREATE INDEX IF NOT EXISTS ix_schedule_blocks_start_at
    ON operations.schedule_blocks (start_at);

CREATE INDEX IF NOT EXISTS ix_schedule_blocks_end_at
    ON operations.schedule_blocks (end_at);

DROP TRIGGER IF EXISTS trg_schedule_blocks_updated_at ON operations.schedule_blocks;
CREATE TRIGGER trg_schedule_blocks_updated_at
BEFORE UPDATE ON operations.schedule_blocks
FOR EACH ROW EXECUTE FUNCTION operations.set_updated_at();

-- =========================================================
-- DISPATCH RULES
-- Company / branch / region assignment logic
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.dispatch_rules (
    dispatch_rule_id          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,

    rule_code                 varchar(50) NOT NULL,
    rule_name                 varchar(200) NOT NULL,
    rule_type                 operations.dispatch_rule_type NOT NULL DEFAULT 'manual_override',

    priority                  integer NOT NULL DEFAULT 100,
    is_active                 boolean NOT NULL DEFAULT true,

    branch_id                 uuid,
    region_id                 uuid,
    state_code                char(2),
    service_type_id           uuid,
    service_mode              operations.service_mode,

    conditions                jsonb NOT NULL DEFAULT '{}'::jsonb,
    actions                   jsonb NOT NULL DEFAULT '{}'::jsonb,

    notes                     text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id        uuid,
    updated_by_user_id        uuid,
    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_dispatch_rules_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_dispatch_rules_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_dispatch_rules_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_dispatch_rules_service_type
        FOREIGN KEY (service_type_id) REFERENCES operations.service_types (service_type_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_dispatch_rules_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_dispatch_rules_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_dispatch_rules_tenant_code
        UNIQUE (tenant_id, rule_code),

    CONSTRAINT chk_dispatch_rules_priority
        CHECK (priority >= 0)
);

CREATE INDEX IF NOT EXISTS ix_dispatch_rules_tenant_id
    ON operations.dispatch_rules (tenant_id);

CREATE INDEX IF NOT EXISTS ix_dispatch_rules_rule_type
    ON operations.dispatch_rules (rule_type);

CREATE INDEX IF NOT EXISTS ix_dispatch_rules_priority
    ON operations.dispatch_rules (priority);

CREATE INDEX IF NOT EXISTS ix_dispatch_rules_is_active
    ON operations.dispatch_rules (is_active);

CREATE INDEX IF NOT EXISTS ix_dispatch_rules_service_type_id
    ON operations.dispatch_rules (service_type_id);

DROP TRIGGER IF EXISTS trg_dispatch_rules_updated_at ON operations.dispatch_rules;
CREATE TRIGGER trg_dispatch_rules_updated_at
BEFORE UPDATE ON operations.dispatch_rules
FOR EACH ROW EXECUTE FUNCTION operations.set_updated_at();

-- =========================================================
-- DISPATCH RUNS
-- Snapshot of each algorithm/run used to evaluate candidates
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.dispatch_runs (
    dispatch_run_id           uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,

    run_code                  varchar(50) NOT NULL,
    job_request_id            uuid NOT NULL,

    algorithm_name            varchar(100),
    algorithm_version         varchar(50),
    status                    operations.dispatch_run_status NOT NULL DEFAULT 'queued',

    initiated_by_user_id      uuid,
    started_at                timestamptz,
    completed_at              timestamptz,
    failed_at                 timestamptz,
    failure_reason            text,

    notes                     text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_dispatch_runs_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_dispatch_runs_job_request
        FOREIGN KEY (job_request_id) REFERENCES operations.job_requests (job_request_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_dispatch_runs_initiated_by
        FOREIGN KEY (initiated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_dispatch_runs_tenant_code
        UNIQUE (tenant_id, run_code)
);

CREATE INDEX IF NOT EXISTS ix_dispatch_runs_tenant_id
    ON operations.dispatch_runs (tenant_id);

CREATE INDEX IF NOT EXISTS ix_dispatch_runs_job_request_id
    ON operations.dispatch_runs (job_request_id);

CREATE INDEX IF NOT EXISTS ix_dispatch_runs_status
    ON operations.dispatch_runs (status);

DROP TRIGGER IF EXISTS trg_dispatch_runs_updated_at ON operations.dispatch_runs;
CREATE TRIGGER trg_dispatch_runs_updated_at
BEFORE UPDATE ON operations.dispatch_runs
FOR EACH ROW EXECUTE FUNCTION operations.set_updated_at();

-- =========================================================
-- DISPATCH CANDIDATES
-- Eligible notaries per dispatch run
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.dispatch_candidates (
    dispatch_candidate_id     uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    dispatch_run_id           uuid NOT NULL,
    notary_id                 uuid NOT NULL,

    candidate_status          operations.dispatch_candidate_status NOT NULL DEFAULT 'eligible',
    rank_no                   integer NOT NULL,
    score                     numeric(12,4),
    distance_km               numeric(10,2),
    travel_minutes            integer,

    branch_id                 uuid,
    region_id                 uuid,
    state_code                char(2),
    service_mode              operations.service_mode,
    service_type_id           uuid,

    is_selected               boolean NOT NULL DEFAULT false,
    selection_reason          text,
    rejection_reason          text,
    compliance_snapshot       jsonb NOT NULL DEFAULT '{}'::jsonb,
    availability_snapshot     jsonb NOT NULL DEFAULT '{}'::jsonb,
    notes                     text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_dispatch_candidates_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_dispatch_candidates_run
        FOREIGN KEY (dispatch_run_id) REFERENCES operations.dispatch_runs (dispatch_run_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_dispatch_candidates_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_dispatch_candidates_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_dispatch_candidates_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_dispatch_candidates_service_type
        FOREIGN KEY (service_type_id) REFERENCES operations.service_types (service_type_id)
        ON DELETE SET NULL,

    CONSTRAINT chk_dispatch_candidates_rank
        CHECK (rank_no >= 0),

    CONSTRAINT chk_dispatch_candidates_score
        CHECK (score IS NULL OR score >= 0),

    CONSTRAINT chk_dispatch_candidates_distance
        CHECK (distance_km IS NULL OR distance_km >= 0),

    CONSTRAINT chk_dispatch_candidates_travel_minutes
        CHECK (travel_minutes IS NULL OR travel_minutes >= 0)
);

CREATE INDEX IF NOT EXISTS ix_dispatch_candidates_tenant_id
    ON operations.dispatch_candidates (tenant_id);

CREATE INDEX IF NOT EXISTS ix_dispatch_candidates_dispatch_run_id
    ON operations.dispatch_candidates (dispatch_run_id);

CREATE INDEX IF NOT EXISTS ix_dispatch_candidates_notary_id
    ON operations.dispatch_candidates (notary_id);

CREATE INDEX IF NOT EXISTS ix_dispatch_candidates_candidate_status
    ON operations.dispatch_candidates (candidate_status);

CREATE INDEX IF NOT EXISTS ix_dispatch_candidates_rank_no
    ON operations.dispatch_candidates (rank_no);

DROP TRIGGER IF EXISTS trg_dispatch_candidates_updated_at ON operations.dispatch_candidates;
CREATE TRIGGER trg_dispatch_candidates_updated_at
BEFORE UPDATE ON operations.dispatch_candidates
FOR EACH ROW EXECUTE FUNCTION operations.set_updated_at();

CREATE UNIQUE INDEX IF NOT EXISTS ux_dispatch_candidates_run_notary
    ON operations.dispatch_candidates (dispatch_run_id, notary_id)
    WHERE deleted_at IS NULL;

-- =========================================================
-- JOB REMINDERS
-- Reminder schedule and delivery tracking
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.job_reminders (
    job_reminder_id           uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    job_id                    uuid NOT NULL,

    reminder_code             varchar(50) NOT NULL,
    scope                     operations.notification_scope NOT NULL DEFAULT 'mixed',
    channel                   operations.reminder_channel NOT NULL,
    status                    operations.reminder_status NOT NULL DEFAULT 'pending',

    recipient_type            operations.reminder_recipient_type NOT NULL,
    recipient_user_id         uuid,
    recipient_contact_id      uuid,
    recipient_notary_id       uuid,

    template_code             varchar(100),
    title                     varchar(200),
    message_body              text,
    payload                   jsonb NOT NULL DEFAULT '{}'::jsonb,

    scheduled_at              timestamptz NOT NULL,
    sent_at                   timestamptz,
    delivered_at              timestamptz,
    failed_at                 timestamptz,
    failure_reason            text,

    external_message_id       text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id        uuid,
    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_job_reminders_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_job_reminders_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_job_reminders_recipient_user
        FOREIGN KEY (recipient_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_job_reminders_recipient_contact
        FOREIGN KEY (recipient_contact_id) REFERENCES crm.customer_contacts (contact_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_job_reminders_recipient_notary
        FOREIGN KEY (recipient_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_job_reminders_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_job_reminders_tenant_code
        UNIQUE (tenant_id, reminder_code)
);

CREATE INDEX IF NOT EXISTS ix_job_reminders_tenant_id
    ON operations.job_reminders (tenant_id);

CREATE INDEX IF NOT EXISTS ix_job_reminders_job_id
    ON operations.job_reminders (job_id);

CREATE INDEX IF NOT EXISTS ix_job_reminders_channel
    ON operations.job_reminders (channel);

CREATE INDEX IF NOT EXISTS ix_job_reminders_status
    ON operations.job_reminders (status);

CREATE INDEX IF NOT EXISTS ix_job_reminders_scheduled_at
    ON operations.job_reminders (scheduled_at);

DROP TRIGGER IF EXISTS trg_job_reminders_updated_at ON operations.job_reminders;
CREATE TRIGGER trg_job_reminders_updated_at
BEFORE UPDATE ON operations.job_reminders
FOR EACH ROW EXECUTE FUNCTION operations.set_updated_at();

-- =========================================================
-- OPERATIONAL REPORT SNAPSHOTS
-- Optional lightweight aggregates for dashboards
-- =========================================================

CREATE TABLE IF NOT EXISTS operations.daily_operational_snapshots (
    daily_snapshot_id         uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    snapshot_date             date NOT NULL,

    branch_id                 uuid,
    region_id                 uuid,

    total_requests            integer NOT NULL DEFAULT 0,
    total_jobs                integer NOT NULL DEFAULT 0,
    completed_jobs            integer NOT NULL DEFAULT 0,
    cancelled_jobs            integer NOT NULL DEFAULT 0,
    failed_jobs               integer NOT NULL DEFAULT 0,
    on_time_jobs              integer NOT NULL DEFAULT 0,
    reassigned_jobs            integer NOT NULL DEFAULT 0,

    total_reminders_sent      integer NOT NULL DEFAULT 0,
    total_dispatch_runs       integer NOT NULL DEFAULT 0,

    metrics                   jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_daily_operational_snapshots_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_daily_operational_snapshots_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_daily_operational_snapshots_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_daily_operational_snapshots
        UNIQUE (tenant_id, snapshot_date, branch_id, region_id),

    CONSTRAINT chk_daily_operational_snapshots_counts
        CHECK (
            total_requests >= 0 AND
            total_jobs >= 0 AND
            completed_jobs >= 0 AND
            cancelled_jobs >= 0 AND
            failed_jobs >= 0 AND
            on_time_jobs >= 0 AND
            reassigned_jobs >= 0 AND
            total_reminders_sent >= 0 AND
            total_dispatch_runs >= 0
        )
);

CREATE INDEX IF NOT EXISTS ix_daily_operational_snapshots_tenant_id
    ON operations.daily_operational_snapshots (tenant_id);

CREATE INDEX IF NOT EXISTS ix_daily_operational_snapshots_snapshot_date
    ON operations.daily_operational_snapshots (snapshot_date);

DROP TRIGGER IF EXISTS trg_daily_operational_snapshots_updated_at ON operations.daily_operational_snapshots;
CREATE TRIGGER trg_daily_operational_snapshots_updated_at
BEFORE UPDATE ON operations.daily_operational_snapshots
FOR EACH ROW EXECUTE FUNCTION operations.set_updated_at();

-- =========================================================
-- VIEWS
-- =========================================================

CREATE OR REPLACE VIEW operations.v_job_board AS
SELECT
    j.job_id,
    j.tenant_id,
    j.job_code,
    j.status,
    j.priority,
    j.service_mode,
    st.service_code,
    st.service_name,
    j.customer_id,
    c.display_name AS customer_name,
    j.branch_id,
    j.region_id,
    j.requested_start_at,
    j.requested_end_at,
    j.scheduled_start_at,
    j.scheduled_end_at,
    j.sla_due_at,
    j.rush_flag,
    j.locked_at,
    j.completed_at,
    j.cancelled_at
FROM operations.jobs j
JOIN operations.service_types st ON st.service_type_id = j.service_type_id
JOIN crm.customers c ON c.customer_id = j.customer_id
WHERE j.deleted_at IS NULL
  AND st.deleted_at IS NULL
  AND c.deleted_at IS NULL;

CREATE OR REPLACE VIEW operations.v_current_primary_assignment AS
SELECT
    ja.job_assignment_id,
    ja.tenant_id,
    ja.job_id,
    ja.notary_id,
    ja.assignment_role,
    ja.status,
    ja.assigned_at,
    ja.accepted_at,
    ja.compliance_snapshot,
    ja.performance_snapshot
FROM operations.job_assignments ja
WHERE ja.deleted_at IS NULL
  AND ja.is_primary = true;

CREATE OR REPLACE VIEW operations.v_notary_daily_schedule AS
SELECT
    sb.schedule_block_id,
    sb.tenant_id,
    sb.notary_id,
    sb.job_id,
    sb.job_assignment_id,
    sb.block_type,
    sb.title,
    sb.start_at,
    sb.end_at,
    sb.timezone,
    sb.location_name,
    sb.is_all_day,
    sb.is_conflict
FROM operations.schedule_blocks sb
WHERE sb.deleted_at IS NULL;

-- =========================================================
-- COMMENTS
-- =========================================================

COMMENT ON SCHEMA operations IS 'Operations schema for request intake, jobs, dispatch, scheduling, availability, and reminders.';
COMMENT ON TABLE operations.service_types IS 'Operational service catalog used by requests, jobs, and dispatch.';
COMMENT ON TABLE operations.job_requests IS 'Incoming job requests and orders.';
COMMENT ON TABLE operations.jobs IS 'Confirmed operational job records.';
COMMENT ON TABLE operations.job_assignments IS 'History of notary assignments for each job.';
COMMENT ON TABLE operations.job_status_history IS 'Immutable status transition log for jobs.';
COMMENT ON TABLE operations.job_timeline_events IS 'Flexible event timeline for the job detail screen.';
COMMENT ON TABLE operations.notary_availability_rules IS 'Recurring availability and blackout rules for notaries.';
COMMENT ON TABLE operations.notary_shift_requests IS 'Time-off, shift change, and blackout requests.';
COMMENT ON TABLE operations.schedule_blocks IS 'Master scheduling board entries and calendar blocks.';
COMMENT ON TABLE operations.dispatch_rules IS 'Rule catalog for dispatch logic.';
COMMENT ON TABLE operations.dispatch_runs IS 'Dispatch evaluation runs and algorithm snapshots.';
COMMENT ON TABLE operations.dispatch_candidates IS 'Candidate notaries per dispatch run.';
COMMENT ON TABLE operations.job_reminders IS 'Operational reminders and delivery tracking.';
COMMENT ON TABLE operations.daily_operational_snapshots IS 'Daily operational dashboard aggregates.';


-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================

-- PostgreSQL 15+
-- Notarial schema for notarial acts, signers, identity verification,
-- execution records, certificates, documents, status history, and audit logs.

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

CREATE SCHEMA IF NOT EXISTS notarial;

-- =========================================================
-- ENUM TYPES
-- =========================================================

DO $$
BEGIN
    IF to_regtype('notarial.notarial_act_type') IS NULL THEN
        CREATE TYPE notarial.notarial_act_type AS ENUM (
            'acknowledgment',
            'jurat',
            'oath_affirmation',
            'copy_certification',
            'signature_witnessing',
            'loan_signing',
            'affidavit',
            'power_of_attorney',
            'ron_acknowledgment',
            'ron_jurat',
            'other'
        );
    END IF;

    IF to_regtype('notarial.notarial_act_status') IS NULL THEN
        CREATE TYPE notarial.notarial_act_status AS ENUM (
            'draft',
            'pending_verification',
            'in_execution',
            'awaiting_certificate',
            'awaiting_journal',
            'completed',
            'locked',
            'voided',
            'cancelled',
            'archived'
        );
    END IF;

    IF to_regtype('notarial.signer_role') IS NULL THEN
        CREATE TYPE notarial.signer_role AS ENUM (
            'signer',
            'principal',
            'witness',
            'agent',
            'attorney_in_fact',
            'translator',
            'other'
        );
    END IF;

    IF to_regtype('notarial.identity_verification_method') IS NULL THEN
        CREATE TYPE notarial.identity_verification_method AS ENUM (
            'physical_presence',
            'government_id',
            'kba',
            'credential_analysis',
            'biometric',
            'video_conference',
            'remote_online_notarization',
            'other'
        );
    END IF;

    IF to_regtype('notarial.verification_result') IS NULL THEN
        CREATE TYPE notarial.verification_result AS ENUM (
            'pending',
            'passed',
            'failed',
            'expired',
            'incomplete',
            'manual_review'
        );
    END IF;

    IF to_regtype('notarial.appearance_type') IS NULL THEN
        CREATE TYPE notarial.appearance_type AS ENUM (
            'physical',
            'remote',
            'hybrid'
        );
    END IF;

    IF to_regtype('notarial.execution_status') IS NULL THEN
        CREATE TYPE notarial.execution_status AS ENUM (
            'not_started',
            'in_progress',
            'completed',
            'abandoned',
            'requires_review'
        );
    END IF;

    IF to_regtype('notarial.certificate_status') IS NULL THEN
        CREATE TYPE notarial.certificate_status AS ENUM (
            'draft',
            'generated',
            'previewed',
            'finalized',
            'locked',
            'voided'
        );
    END IF;

    IF to_regtype('notarial.document_link_type') IS NULL THEN
        CREATE TYPE notarial.document_link_type AS ENUM (
            'subject_document',
            'supporting_document',
            'id_document',
            'attachment',
            'evidence',
            'other'
        );
    END IF;

    IF to_regtype('notarial.act_event_type') IS NULL THEN
        CREATE TYPE notarial.act_event_type AS ENUM (
            'create',
            'update',
            'status_change',
            'verification_added',
            'execution_started',
            'execution_completed',
            'certificate_generated',
            'certificate_finalized',
            'journal_linked',
            'locked',
            'voided',
            'unlocked',
            'document_attached',
            'document_removed'
        );
    END IF;
END $$;

-- =========================================================
-- COMMON UPDATED_AT TRIGGER
-- =========================================================

CREATE OR REPLACE FUNCTION notarial.set_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at = now();
    RETURN NEW;
END;
$$;

-- =========================================================
-- NOTARIAL ACTS
-- Central legal transaction record
-- =========================================================

CREATE TABLE IF NOT EXISTS notarial.notarial_acts (
    act_id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,

    act_code                  varchar(50) NOT NULL,
    act_type                  notarial.notarial_act_type NOT NULL,
    status                    notarial.notarial_act_status NOT NULL DEFAULT 'draft',

    job_id                    uuid,
    job_request_id            uuid,
    customer_id               uuid NOT NULL,
    customer_contact_id       uuid,
    notary_id                 uuid NOT NULL,

    state_code                char(2) NOT NULL,
    venue_name                varchar(200),
    venue_address_line1       varchar(200),
    venue_address_line2       varchar(200),
    venue_city                varchar(100),
    venue_state_code          char(2),
    venue_postal_code         varchar(20),
    venue_country_code        char(2),
    latitude                  numeric(10,7),
    longitude                 numeric(10,7),

    document_title            varchar(300),
    document_page_count       integer,
    document_reference_no     varchar(100),
    signer_count              integer NOT NULL DEFAULT 1,

    appearance_type           notarial.appearance_type NOT NULL DEFAULT 'physical',
    oath_required             boolean NOT NULL DEFAULT false,
    thumbprint_required       boolean NOT NULL DEFAULT false,
    identity_verification_required boolean NOT NULL DEFAULT true,

    personal_appearance_confirmed boolean NOT NULL DEFAULT false,
    oath_administered         boolean NOT NULL DEFAULT false,

    act_started_at            timestamptz,
    act_completed_at          timestamptz,
    act_locked_at             timestamptz,
    act_locked_by_user_id     uuid,
    act_voided_at             timestamptz,
    act_voided_by_user_id     uuid,
    void_reason               text,

    linked_journal_entry_id   uuid,
    linked_certificate_id     uuid,

    notes                     text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id        uuid,
    updated_by_user_id        uuid,
    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_notarial_acts_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_notarial_acts_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_notarial_acts_job_request
        FOREIGN KEY (job_request_id) REFERENCES operations.job_requests (job_request_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_notarial_acts_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_notarial_acts_customer_contact
        FOREIGN KEY (customer_contact_id) REFERENCES crm.customer_contacts (contact_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_notarial_acts_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_notarial_acts_locked_by_user
        FOREIGN KEY (act_locked_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_notarial_acts_voided_by_user
        FOREIGN KEY (act_voided_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_notarial_acts_created_by_user
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_notarial_acts_updated_by_user
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_notarial_acts_tenant_code
        UNIQUE (tenant_id, act_code),

    CONSTRAINT chk_notarial_acts_signer_count
        CHECK (signer_count >= 1),

    CONSTRAINT chk_notarial_acts_document_page_count
        CHECK (document_page_count IS NULL OR document_page_count >= 0)
);

CREATE INDEX IF NOT EXISTS ix_notarial_acts_tenant_id
    ON notarial.notarial_acts (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notarial_acts_job_id
    ON notarial.notarial_acts (job_id);

CREATE INDEX IF NOT EXISTS ix_notarial_acts_customer_id
    ON notarial.notarial_acts (customer_id);

CREATE INDEX IF NOT EXISTS ix_notarial_acts_notary_id
    ON notarial.notarial_acts (notary_id);

CREATE INDEX IF NOT EXISTS ix_notarial_acts_status
    ON notarial.notarial_acts (status);

CREATE INDEX IF NOT EXISTS ix_notarial_acts_act_type
    ON notarial.notarial_acts (act_type);

CREATE INDEX IF NOT EXISTS ix_notarial_acts_state_code
    ON notarial.notarial_acts (state_code);

CREATE INDEX IF NOT EXISTS ix_notarial_acts_act_completed_at
    ON notarial.notarial_acts (act_completed_at);

DROP TRIGGER IF EXISTS trg_notarial_acts_updated_at ON notarial.notarial_acts;
CREATE TRIGGER trg_notarial_acts_updated_at
BEFORE UPDATE ON notarial.notarial_acts
FOR EACH ROW EXECUTE FUNCTION notarial.set_updated_at();

-- =========================================================
-- ACT SIGNERS
-- One act can have multiple signers
-- =========================================================

CREATE TABLE IF NOT EXISTS notarial.act_signers (
    act_signer_id             uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    act_id                    uuid NOT NULL,

    signer_order              integer NOT NULL DEFAULT 1,
    signer_role               notarial.signer_role NOT NULL DEFAULT 'signer',

    full_legal_name           varchar(200) NOT NULL,
    capacity_or_title         varchar(150),
    is_principal              boolean NOT NULL DEFAULT false,
    is_primary_signer         boolean NOT NULL DEFAULT false,

    email                     citext,
    phone                     varchar(30),
    address_line1             varchar(200),
    address_line2             varchar(200),
    city                      varchar(100),
    state_code                char(2),
    postal_code               varchar(20),
    country_code              char(2),

    id_type                   varchar(100),
    id_number                 varchar(100),
    issuing_authority         varchar(200),
    id_issue_date             date,
    id_expiration_date        date,

    appearance_type           notarial.appearance_type NOT NULL DEFAULT 'physical',
    appearance_confirmed      boolean NOT NULL DEFAULT false,

    signature_required        boolean NOT NULL DEFAULT true,
    signature_captured        boolean NOT NULL DEFAULT false,
    signature_captured_at     timestamptz,

    thumbprint_required       boolean NOT NULL DEFAULT false,
    thumbprint_captured       boolean NOT NULL DEFAULT false,
    thumbprint_captured_at    timestamptz,

    notes                     text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_act_signers_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_act_signers_act
        FOREIGN KEY (act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE CASCADE,

    CONSTRAINT chk_act_signers_order
        CHECK (signer_order >= 1),

    CONSTRAINT chk_act_signers_id_dates
        CHECK (id_expiration_date IS NULL OR id_issue_date IS NULL OR id_expiration_date >= id_issue_date)
);

CREATE INDEX IF NOT EXISTS ix_act_signers_tenant_id
    ON notarial.act_signers (tenant_id);

CREATE INDEX IF NOT EXISTS ix_act_signers_act_id
    ON notarial.act_signers (act_id);

CREATE INDEX IF NOT EXISTS ix_act_signers_full_legal_name
    ON notarial.act_signers (full_legal_name);

CREATE INDEX IF NOT EXISTS ix_act_signers_signer_role
    ON notarial.act_signers (signer_role);

CREATE INDEX IF NOT EXISTS ix_act_signers_is_primary_signer
    ON notarial.act_signers (is_primary_signer);

DROP TRIGGER IF EXISTS trg_act_signers_updated_at ON notarial.act_signers;
CREATE TRIGGER trg_act_signers_updated_at
BEFORE UPDATE ON notarial.act_signers
FOR EACH ROW EXECUTE FUNCTION notarial.set_updated_at();

-- Only one primary signer per act
CREATE UNIQUE INDEX IF NOT EXISTS ux_act_signers_one_primary
    ON notarial.act_signers (act_id)
    WHERE is_primary_signer = true AND deleted_at IS NULL;

-- =========================================================
-- ACT IDENTITY VERIFICATIONS
-- Can store multiple verification steps per signer
-- =========================================================

CREATE TABLE IF NOT EXISTS notarial.act_identity_verifications (
    verification_id           uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    act_id                    uuid NOT NULL,
    act_signer_id             uuid NOT NULL,

    verification_method       notarial.identity_verification_method NOT NULL,
    result                    notarial.verification_result NOT NULL DEFAULT 'pending',

    verified_by_user_id       uuid,
    verified_by_notary_id     uuid,

    verified_at               timestamptz,
    kba_attempt_count         integer NOT NULL DEFAULT 0,
    kba_score                 numeric(5,2),
    credential_analysis_score numeric(5,2),

    id_type                   varchar(100),
    id_number                 varchar(100),
    issuing_authority         varchar(200),
    id_issue_date             date,
    id_expiration_date        date,

    id_front_file_id          uuid,
    id_back_file_id           uuid,
    selfie_file_id            uuid,

    failure_reason            text,
    review_notes              text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_act_identity_verifications_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_act_identity_verifications_act
        FOREIGN KEY (act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_act_identity_verifications_signer
        FOREIGN KEY (act_signer_id) REFERENCES notarial.act_signers (act_signer_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_act_identity_verifications_verified_by_user
        FOREIGN KEY (verified_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_act_identity_verifications_verified_by_notary
        FOREIGN KEY (verified_by_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT chk_act_identity_verifications_kba_attempts
        CHECK (kba_attempt_count >= 0),

    CONSTRAINT chk_act_identity_verifications_scores
        CHECK (
            (kba_score IS NULL OR (kba_score >= 0 AND kba_score <= 100)) AND
            (credential_analysis_score IS NULL OR (credential_analysis_score >= 0 AND credential_analysis_score <= 100))
        )
);

CREATE INDEX IF NOT EXISTS ix_act_identity_verifications_tenant_id
    ON notarial.act_identity_verifications (tenant_id);

CREATE INDEX IF NOT EXISTS ix_act_identity_verifications_act_id
    ON notarial.act_identity_verifications (act_id);

CREATE INDEX IF NOT EXISTS ix_act_identity_verifications_act_signer_id
    ON notarial.act_identity_verifications (act_signer_id);

CREATE INDEX IF NOT EXISTS ix_act_identity_verifications_result
    ON notarial.act_identity_verifications (result);

CREATE INDEX IF NOT EXISTS ix_act_identity_verifications_method
    ON notarial.act_identity_verifications (verification_method);

DROP TRIGGER IF EXISTS trg_act_identity_verifications_updated_at ON notarial.act_identity_verifications;
CREATE TRIGGER trg_act_identity_verifications_updated_at
BEFORE UPDATE ON notarial.act_identity_verifications
FOR EACH ROW EXECUTE FUNCTION notarial.set_updated_at();

-- =========================================================
-- ACT DOCUMENTS
-- Supporting documents, attachments, ID images, evidence files
-- =========================================================

CREATE TABLE IF NOT EXISTS notarial.act_documents (
    act_document_id           uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    act_id                    uuid NOT NULL,

    document_link_type        notarial.document_link_type NOT NULL DEFAULT 'supporting_document',
    file_name                 varchar(255) NOT NULL,
    file_extension            varchar(20),
    mime_type                 varchar(100),
    storage_provider          varchar(50) NOT NULL DEFAULT 'object_storage',
    storage_key               text NOT NULL,
    file_size_bytes           bigint,
    checksum_sha256           char(64),

    title                     varchar(300),
    description               text,
    is_sensitive              boolean NOT NULL DEFAULT false,
    visibility_level          varchar(50) NOT NULL DEFAULT 'restricted',

    uploaded_by_user_id       uuid,
    uploaded_at               timestamptz NOT NULL DEFAULT now(),

    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_act_documents_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_act_documents_act
        FOREIGN KEY (act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_act_documents_uploaded_by
        FOREIGN KEY (uploaded_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT chk_act_documents_file_size
        CHECK (file_size_bytes IS NULL OR file_size_bytes >= 0)
);

CREATE INDEX IF NOT EXISTS ix_act_documents_tenant_id
    ON notarial.act_documents (tenant_id);

CREATE INDEX IF NOT EXISTS ix_act_documents_act_id
    ON notarial.act_documents (act_id);

CREATE INDEX IF NOT EXISTS ix_act_documents_document_link_type
    ON notarial.act_documents (document_link_type);

CREATE INDEX IF NOT EXISTS ix_act_documents_uploaded_at
    ON notarial.act_documents (uploaded_at);

DROP TRIGGER IF EXISTS trg_act_documents_updated_at ON notarial.act_documents;
CREATE TRIGGER trg_act_documents_updated_at
BEFORE UPDATE ON notarial.act_documents
FOR EACH ROW EXECUTE FUNCTION notarial.set_updated_at();

-- =========================================================
-- ACT EXECUTION RECORDS
-- Legal execution steps, oath, appearance, notes, timestamps
-- =========================================================

CREATE TABLE IF NOT EXISTS notarial.act_execution_records (
    execution_record_id       uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    act_id                    uuid NOT NULL,

    execution_status          notarial.execution_status NOT NULL DEFAULT 'not_started',
    started_at                timestamptz,
    completed_at              timestamptz,

    executed_by_user_id       uuid,
    executed_by_notary_id     uuid NOT NULL,

    personal_appearance_confirmed boolean NOT NULL DEFAULT false,
    oath_administered         boolean NOT NULL DEFAULT false,
    oath_text                 text,

    signature_capture_method   varchar(100),
    signature_captured         boolean NOT NULL DEFAULT false,
    signature_captured_at      timestamptz,

    notes                     text,
    observations              text,
    exception_flags           jsonb NOT NULL DEFAULT '{}'::jsonb,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_act_execution_records_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_act_execution_records_act
        FOREIGN KEY (act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_act_execution_records_executed_by_user
        FOREIGN KEY (executed_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_act_execution_records_executed_by_notary
        FOREIGN KEY (executed_by_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS ix_act_execution_records_tenant_id
    ON notarial.act_execution_records (tenant_id);

CREATE INDEX IF NOT EXISTS ix_act_execution_records_act_id
    ON notarial.act_execution_records (act_id);

CREATE INDEX IF NOT EXISTS ix_act_execution_records_execution_status
    ON notarial.act_execution_records (execution_status);

CREATE INDEX IF NOT EXISTS ix_act_execution_records_executed_by_notary_id
    ON notarial.act_execution_records (executed_by_notary_id);

DROP TRIGGER IF EXISTS trg_act_execution_records_updated_at ON notarial.act_execution_records;
CREATE TRIGGER trg_act_execution_records_updated_at
BEFORE UPDATE ON notarial.act_execution_records
FOR EACH ROW EXECUTE FUNCTION notarial.set_updated_at();

-- =========================================================
-- NOTARIAL CERTIFICATES
-- Generated legal certificate content and lock state
-- =========================================================

CREATE TABLE IF NOT EXISTS notarial.notarial_certificates (
    certificate_id            uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    act_id                    uuid NOT NULL,

    certificate_number        varchar(100) NOT NULL,
    certificate_status        notarial.certificate_status NOT NULL DEFAULT 'draft',

    certificate_template_code  varchar(100),
    certificate_title         varchar(300),
    certificate_body          text NOT NULL,

    venue_text                varchar(300),
    state_code                char(2) NOT NULL,
    county_name               varchar(100),
    city_name                 varchar(100),
    issue_date                date,
    issue_time                time,

    seal_reference            text,
    digital_certificate_reference text,
    cryptographic_signature   text,

    generated_at              timestamptz,
    previewed_at              timestamptz,
    finalized_at              timestamptz,
    locked_at                 timestamptz,

    generated_by_user_id      uuid,
    finalized_by_user_id      uuid,
    locked_by_user_id         uuid,

    linked_journal_entry_id   uuid,
    notes                     text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_notarial_certificates_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_notarial_certificates_act
        FOREIGN KEY (act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_notarial_certificates_generated_by
        FOREIGN KEY (generated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_notarial_certificates_finalized_by
        FOREIGN KEY (finalized_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_notarial_certificates_locked_by
        FOREIGN KEY (locked_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_notarial_certificates_act_number
        UNIQUE (act_id, certificate_number)
);

CREATE INDEX IF NOT EXISTS ix_notarial_certificates_tenant_id
    ON notarial.notarial_certificates (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notarial_certificates_act_id
    ON notarial.notarial_certificates (act_id);

CREATE INDEX IF NOT EXISTS ix_notarial_certificates_status
    ON notarial.notarial_certificates (certificate_status);

CREATE INDEX IF NOT EXISTS ix_notarial_certificates_state_code
    ON notarial.notarial_certificates (state_code);

CREATE INDEX IF NOT EXISTS ix_notarial_certificates_finalized_at
    ON notarial.notarial_certificates (finalized_at);

DROP TRIGGER IF EXISTS trg_notarial_certificates_updated_at ON notarial.notarial_certificates;
CREATE TRIGGER trg_notarial_certificates_updated_at
BEFORE UPDATE ON notarial.notarial_certificates
FOR EACH ROW EXECUTE FUNCTION notarial.set_updated_at();

-- link back to act after certificate exists
ALTER TABLE notarial.notarial_acts
    ADD CONSTRAINT fk_notarial_acts_linked_certificate
    FOREIGN KEY (linked_certificate_id)
    REFERENCES notarial.notarial_certificates (certificate_id)
    ON DELETE SET NULL;

-- optional journal link is kept as UUID because journal schema may be created later
-- and to avoid circular dependency here.

-- =========================================================
-- ACT STATUS HISTORY
-- Immutable state transitions for audit and legal traceability
-- =========================================================

CREATE TABLE IF NOT EXISTS notarial.act_status_history (
    act_status_history_id     uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    act_id                    uuid NOT NULL,

    previous_status           notarial.notarial_act_status,
    new_status                notarial.notarial_act_status NOT NULL,
    reason                    text,
    source_reference          text,

    changed_by_user_id        uuid,
    changed_by_notary_id      uuid,
    effective_at              timestamptz NOT NULL DEFAULT now(),

    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at                timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_act_status_history_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_act_status_history_act
        FOREIGN KEY (act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_act_status_history_changed_by_user
        FOREIGN KEY (changed_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_act_status_history_changed_by_notary
        FOREIGN KEY (changed_by_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_act_status_history_tenant_id
    ON notarial.act_status_history (tenant_id);

CREATE INDEX IF NOT EXISTS ix_act_status_history_act_id
    ON notarial.act_status_history (act_id);

CREATE INDEX IF NOT EXISTS ix_act_status_history_effective_at
    ON notarial.act_status_history (effective_at);

CREATE INDEX IF NOT EXISTS ix_act_status_history_new_status
    ON notarial.act_status_history (new_status);

-- =========================================================
-- ACT AUDIT LOGS
-- Fine-grained evidence-grade log
-- =========================================================

CREATE TABLE IF NOT EXISTS notarial.act_audit_logs (
    audit_log_id              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    act_id                    uuid NOT NULL,

    event_type                notarial.act_event_type NOT NULL,
    event_title               varchar(200),
    event_body                text,

    actor_user_id             uuid,
    actor_notary_id           uuid,
    source_ip                 inet,
    user_agent                text,

    before_data               jsonb,
    after_data                jsonb,
    occurred_at               timestamptz NOT NULL DEFAULT now(),
    source_reference          text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_act_audit_logs_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_act_audit_logs_act
        FOREIGN KEY (act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_act_audit_logs_actor_user
        FOREIGN KEY (actor_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_act_audit_logs_actor_notary
        FOREIGN KEY (actor_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_act_audit_logs_tenant_id
    ON notarial.act_audit_logs (tenant_id);

CREATE INDEX IF NOT EXISTS ix_act_audit_logs_act_id
    ON notarial.act_audit_logs (act_id);

CREATE INDEX IF NOT EXISTS ix_act_audit_logs_event_type
    ON notarial.act_audit_logs (event_type);

CREATE INDEX IF NOT EXISTS ix_act_audit_logs_occurred_at
    ON notarial.act_audit_logs (occurred_at);

-- =========================================================
-- OPTIONAL SUPPORTING TABLE: VOID REASONS CATALOG
-- =========================================================

CREATE TABLE IF NOT EXISTS notarial.void_reasons (
    void_reason_id            uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    reason_code               varchar(50) NOT NULL,
    reason_name               varchar(200) NOT NULL,
    description               text,
    is_active                 boolean NOT NULL DEFAULT true,
    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_void_reasons_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT uq_void_reasons_tenant_code
        UNIQUE (tenant_id, reason_code)
);

CREATE INDEX IF NOT EXISTS ix_void_reasons_tenant_id
    ON notarial.void_reasons (tenant_id);

DROP TRIGGER IF EXISTS trg_void_reasons_updated_at ON notarial.void_reasons;
CREATE TRIGGER trg_void_reasons_updated_at
BEFORE UPDATE ON notarial.void_reasons
FOR EACH ROW EXECUTE FUNCTION notarial.set_updated_at();

-- =========================================================
-- VIEWS
-- =========================================================

CREATE OR REPLACE VIEW notarial.v_act_overview AS
SELECT
    a.act_id,
    a.tenant_id,
    a.act_code,
    a.act_type,
    a.status,
    a.state_code,
    a.appearance_type,
    a.oath_required,
    a.thumbprint_required,
    a.identity_verification_required,
    a.personal_appearance_confirmed,
    a.oath_administered,
    a.act_started_at,
    a.act_completed_at,
    a.act_locked_at,
    a.act_voided_at,
    c.display_name AS customer_name,
    n.public_display_name AS notary_name,
    j.job_code
FROM notarial.notarial_acts a
JOIN crm.customers c ON c.customer_id = a.customer_id
JOIN identity.notaries n ON n.notary_id = a.notary_id
LEFT JOIN operations.jobs j ON j.job_id = a.job_id
WHERE a.deleted_at IS NULL
  AND c.deleted_at IS NULL
  AND n.deleted_at IS NULL
  AND (j.deleted_at IS NULL OR j.job_id IS NULL);

CREATE OR REPLACE VIEW notarial.v_act_latest_certificate AS
SELECT DISTINCT ON (c.act_id)
    c.certificate_id,
    c.tenant_id,
    c.act_id,
    c.certificate_number,
    c.certificate_status,
    c.certificate_title,
    c.state_code,
    c.finalized_at,
    c.locked_at
FROM notarial.notarial_certificates c
WHERE c.deleted_at IS NULL
ORDER BY c.act_id, c.created_at DESC;

-- =========================================================
-- COMMENTS
-- =========================================================

COMMENT ON SCHEMA notarial IS 'Notarial core legal schema for acts, signers, verification, execution, certificates, and audit.';
COMMENT ON TABLE notarial.notarial_acts IS 'Central legal transaction record for each notarial act.';
COMMENT ON TABLE notarial.act_signers IS 'One or more signers associated with a notarial act.';
COMMENT ON TABLE notarial.act_identity_verifications IS 'Identity verification steps and evidence for signers.';
COMMENT ON TABLE notarial.act_documents IS 'Supporting documents and uploaded evidence for an act.';
COMMENT ON TABLE notarial.act_execution_records IS 'Execution-stage legal record, including oath and appearance confirmation.';
COMMENT ON TABLE notarial.notarial_certificates IS 'Generated notarial certificate content and lock state.';
COMMENT ON TABLE notarial.act_status_history IS 'Immutable status transitions for legal traceability.';
COMMENT ON TABLE notarial.act_audit_logs IS 'Evidence-grade audit log for every significant act change.';
COMMENT ON TABLE notarial.void_reasons IS 'Catalog of allowed void reasons.';


-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================


-- PostgreSQL 15+
-- Journal schema for notary journal entries, signer identity data,
-- signatures/thumbprints, audit trail, retention, export, and transfer logs.

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

CREATE SCHEMA IF NOT EXISTS journal;

-- =========================================================
-- ENUM TYPES
-- =========================================================

DO $$
BEGIN
    IF to_regtype('journal.journal_entry_status') IS NULL THEN
        CREATE TYPE journal.journal_entry_status AS ENUM (
            'draft',
            'pending',
            'completed',
            'locked',
            'voided',
            'archived'
        );
    END IF;

    IF to_regtype('journal.journal_field_source') IS NULL THEN
        CREATE TYPE journal.journal_field_source AS ENUM (
            'auto_populated',
            'manual',
            'imported',
            'system_generated'
        );
    END IF;

    IF to_regtype('journal.journal_verification_method') IS NULL THEN
        CREATE TYPE journal.journal_verification_method AS ENUM (
            'physical_presence',
            'remote_online_notarization',
            'kba',
            'id_scan',
            'credential_analysis',
            'other'
        );
    END IF;

    IF to_regtype('journal.journal_capture_type') IS NULL THEN
        CREATE TYPE journal.journal_capture_type AS ENUM (
            'signature',
            'thumbprint',
            'both'
        );
    END IF;

    IF to_regtype('journal.journal_export_format') IS NULL THEN
        CREATE TYPE journal.journal_export_format AS ENUM (
            'pdf',
            'csv',
            'xlsx',
            'json'
        );
    END IF;

    IF to_regtype('journal.journal_export_status') IS NULL THEN
        CREATE TYPE journal.journal_export_status AS ENUM (
            'queued',
            'generated',
            'failed',
            'downloaded',
            'expired'
        );
    END IF;

    IF to_regtype('journal.journal_transfer_type') IS NULL THEN
        CREATE TYPE journal.journal_transfer_type AS ENUM (
            'internal_custodian',
            'regulator',
            'successor_notary',
            'archive',
            'legal_hold'
        );
    END IF;

    IF to_regtype('journal.journal_audit_event_type') IS NULL THEN
        CREATE TYPE journal.journal_audit_event_type AS ENUM (
            'create',
            'update',
            'status_change',
            'lock',
            'unlock',
            'void',
            'export',
            'transfer',
            'import',
            'attachment_added',
            'attachment_removed',
            'field_changed',
            'retention_applied'
        );
    END IF;
END $$;

-- =========================================================
-- COMMON UPDATED_AT TRIGGER
-- =========================================================

CREATE OR REPLACE FUNCTION journal.set_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at = now();
    RETURN NEW;
END;
$$;

-- =========================================================
-- IMMUTABILITY GUARD
-- Once a journal entry is locked, it cannot be updated or deleted.
-- =========================================================

CREATE OR REPLACE FUNCTION journal.prevent_locked_journal_modification()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    IF OLD.locked_at IS NOT NULL OR OLD.entry_status = 'locked' THEN
        RAISE EXCEPTION 'Journal entry % is locked and cannot be modified.', OLD.journal_entry_id;
    END IF;

    RETURN NEW;
END;
$$;

CREATE OR REPLACE FUNCTION journal.prevent_locked_journal_deletion()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    IF OLD.locked_at IS NOT NULL OR OLD.entry_status = 'locked' THEN
        RAISE EXCEPTION 'Journal entry % is locked and cannot be deleted.', OLD.journal_entry_id;
    END IF;

    RETURN OLD;
END;
$$;

-- =========================================================
-- JOURNAL ENTRIES
-- One entry per notarial act, immutable once locked
-- =========================================================

CREATE TABLE IF NOT EXISTS journal.journal_entries (
    journal_entry_id          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,

    entry_code                varchar(50) NOT NULL,
    entry_status              journal.journal_entry_status NOT NULL DEFAULT 'draft',

    act_id                    uuid NOT NULL,
    notary_id                 uuid NOT NULL,
    branch_id                 uuid,
    region_id                 uuid,

    state_code                char(2) NOT NULL,
    venue_name                varchar(200),
    venue_address_line1       varchar(200),
    venue_address_line2       varchar(200),
    venue_city                varchar(100),
    venue_state_code          char(2),
    venue_postal_code         varchar(20),
    venue_country_code        char(2),

    entry_date                date NOT NULL,
    entry_time                time NOT NULL,
    entry_timestamp           timestamptz NOT NULL DEFAULT now(),

    act_type                  varchar(100),
    signer_count              integer NOT NULL DEFAULT 1,
    fee_charged               numeric(18,2) NOT NULL DEFAULT 0,
    currency_code             char(3) NOT NULL DEFAULT 'USD',

    source_channel            varchar(50),
    source_reference          text,

    field_source_summary      jsonb NOT NULL DEFAULT '{}'::jsonb,
    compliance_flags          jsonb NOT NULL DEFAULT '{}'::jsonb,
    notes                     text,

    is_missing_thumbprint     boolean NOT NULL DEFAULT false,
    is_missing_signature      boolean NOT NULL DEFAULT false,
    is_complete               boolean NOT NULL DEFAULT false,
    is_locked                 boolean NOT NULL DEFAULT false,

    completed_at              timestamptz,
    locked_at                 timestamptz,
    locked_by_user_id         uuid,
    lock_reason               text,

    voided_at                 timestamptz,
    voided_by_user_id         uuid,
    void_reason               text,

    linked_notarial_act_id    uuid NOT NULL,
    linked_certificate_id     uuid,

    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id        uuid,
    updated_by_user_id        uuid,
    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_journal_entries_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_entries_act
        FOREIGN KEY (act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_entries_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_entries_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_journal_entries_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_journal_entries_locked_by_user
        FOREIGN KEY (locked_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_journal_entries_voided_by_user
        FOREIGN KEY (voided_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_journal_entries_created_by_user
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_journal_entries_updated_by_user
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_journal_entries_linked_act
        FOREIGN KEY (linked_notarial_act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_entries_linked_certificate
        FOREIGN KEY (linked_certificate_id) REFERENCES notarial.notarial_certificates (certificate_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_journal_entries_tenant_code
        UNIQUE (tenant_id, entry_code),

    CONSTRAINT uq_journal_entries_act
        UNIQUE (act_id),

    CONSTRAINT chk_journal_entries_signer_count
        CHECK (signer_count >= 1),

    CONSTRAINT chk_journal_entries_fee
        CHECK (fee_charged >= 0)
);

CREATE INDEX IF NOT EXISTS ix_journal_entries_tenant_id
    ON journal.journal_entries (tenant_id);

CREATE INDEX IF NOT EXISTS ix_journal_entries_act_id
    ON journal.journal_entries (act_id);

CREATE INDEX IF NOT EXISTS ix_journal_entries_notary_id
    ON journal.journal_entries (notary_id);

CREATE INDEX IF NOT EXISTS ix_journal_entries_status
    ON journal.journal_entries (entry_status);

CREATE INDEX IF NOT EXISTS ix_journal_entries_state_code
    ON journal.journal_entries (state_code);

CREATE INDEX IF NOT EXISTS ix_journal_entries_entry_timestamp
    ON journal.journal_entries (entry_timestamp);

DROP TRIGGER IF EXISTS trg_journal_entries_updated_at ON journal.journal_entries;
CREATE TRIGGER trg_journal_entries_updated_at
BEFORE UPDATE ON journal.journal_entries
FOR EACH ROW EXECUTE FUNCTION journal.set_updated_at();

DROP TRIGGER IF EXISTS trg_journal_entries_prevent_locked_update ON journal.journal_entries;
CREATE TRIGGER trg_journal_entries_prevent_locked_update
BEFORE UPDATE ON journal.journal_entries
FOR EACH ROW EXECUTE FUNCTION journal.prevent_locked_journal_modification();

DROP TRIGGER IF EXISTS trg_journal_entries_prevent_locked_delete ON journal.journal_entries;
CREATE TRIGGER trg_journal_entries_prevent_locked_delete
BEFORE DELETE ON journal.journal_entries
FOR EACH ROW EXECUTE FUNCTION journal.prevent_locked_journal_deletion();

-- =========================================================
-- JOURNAL ENTRY SIGNERS
-- Identity and signer details required by state law
-- =========================================================

CREATE TABLE IF NOT EXISTS journal.journal_entry_signers (
    journal_entry_signer_id   uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    journal_entry_id          uuid NOT NULL,

    signer_order              integer NOT NULL DEFAULT 1,
    full_legal_name           varchar(200) NOT NULL,
    role_or_capacity          varchar(150),
    signer_role               varchar(100),

    address_line1             varchar(200),
    address_line2             varchar(200),
    city                      varchar(100),
    state_code                char(2),
    postal_code               varchar(20),
    country_code              char(2),

    id_type                   varchar(100),
    id_number                 varchar(100),
    issuing_authority         varchar(200),
    id_issue_date             date,
    id_expiration_date        date,

    verification_method       journal.journal_verification_method NOT NULL DEFAULT 'physical_presence',
    verification_result       varchar(50) NOT NULL DEFAULT 'pending',

    signer_email              citext,
    signer_phone              varchar(30),

    is_primary_signer         boolean NOT NULL DEFAULT false,
    is_missing_signature      boolean NOT NULL DEFAULT false,
    is_missing_thumbprint     boolean NOT NULL DEFAULT false,

    notes                     text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_journal_entry_signers_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_entry_signers_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE CASCADE,

    CONSTRAINT chk_journal_entry_signers_order
        CHECK (signer_order >= 1),

    CONSTRAINT chk_journal_entry_signers_id_dates
        CHECK (id_expiration_date IS NULL OR id_issue_date IS NULL OR id_expiration_date >= id_issue_date)
);

CREATE INDEX IF NOT EXISTS ix_journal_entry_signers_tenant_id
    ON journal.journal_entry_signers (tenant_id);

CREATE INDEX IF NOT EXISTS ix_journal_entry_signers_journal_entry_id
    ON journal.journal_entry_signers (journal_entry_id);

CREATE INDEX IF NOT EXISTS ix_journal_entry_signers_full_legal_name
    ON journal.journal_entry_signers (full_legal_name);

CREATE INDEX IF NOT EXISTS ix_journal_entry_signers_is_primary_signer
    ON journal.journal_entry_signers (is_primary_signer);

DROP TRIGGER IF EXISTS trg_journal_entry_signers_updated_at ON journal.journal_entry_signers;
CREATE TRIGGER trg_journal_entry_signers_updated_at
BEFORE UPDATE ON journal.journal_entry_signers
FOR EACH ROW EXECUTE FUNCTION journal.set_updated_at();

CREATE UNIQUE INDEX IF NOT EXISTS ux_journal_entry_one_primary_signer
    ON journal.journal_entry_signers (journal_entry_id)
    WHERE is_primary_signer = true AND deleted_at IS NULL;

-- =========================================================
-- JOURNAL ENTRY ID DOCUMENTS
-- ID scans or supporting identity evidence for signer
-- =========================================================

CREATE TABLE IF NOT EXISTS journal.journal_entry_id_documents (
    journal_entry_id_document_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    journal_entry_signer_id    uuid NOT NULL,

    document_type             varchar(100) NOT NULL,
    side                      varchar(20), -- front/back/other
    file_name                 varchar(255) NOT NULL,
    file_extension            varchar(20),
    mime_type                 varchar(100),
    storage_provider          varchar(50) NOT NULL DEFAULT 'object_storage',
    storage_key               text NOT NULL,
    file_size_bytes           bigint,
    checksum_sha256           char(64),

    captured_at               timestamptz NOT NULL DEFAULT now(),
    captured_by_user_id       uuid,
    visibility_level          varchar(50) NOT NULL DEFAULT 'restricted',
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_journal_entry_id_documents_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_entry_id_documents_signer
        FOREIGN KEY (journal_entry_signer_id) REFERENCES journal.journal_entry_signers (journal_entry_signer_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_journal_entry_id_documents_captured_by
        FOREIGN KEY (captured_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT chk_journal_entry_id_documents_size
        CHECK (file_size_bytes IS NULL OR file_size_bytes >= 0)
);

CREATE INDEX IF NOT EXISTS ix_journal_entry_id_documents_tenant_id
    ON journal.journal_entry_id_documents (tenant_id);

CREATE INDEX IF NOT EXISTS ix_journal_entry_id_documents_signer_id
    ON journal.journal_entry_id_documents (journal_entry_signer_id);

CREATE INDEX IF NOT EXISTS ix_journal_entry_id_documents_captured_at
    ON journal.journal_entry_id_documents (captured_at);

DROP TRIGGER IF EXISTS trg_journal_entry_id_documents_updated_at ON journal.journal_entry_id_documents;
CREATE TRIGGER trg_journal_entry_id_documents_updated_at
BEFORE UPDATE ON journal.journal_entry_id_documents
FOR EACH ROW EXECUTE FUNCTION journal.set_updated_at();

-- =========================================================
-- JOURNAL ENTRY SIGNATURES
-- Signature capture metadata and storage
-- =========================================================

CREATE TABLE IF NOT EXISTS journal.journal_entry_signatures (
    journal_entry_signature_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    journal_entry_signer_id   uuid NOT NULL,

    capture_type              journal.journal_capture_type NOT NULL DEFAULT 'signature',
    signature_method          varchar(100),
    signature_format          varchar(50) NOT NULL DEFAULT 'electronic',

    file_name                 varchar(255),
    mime_type                 varchar(100),
    storage_provider          varchar(50) NOT NULL DEFAULT 'object_storage',
    storage_key               text,
    file_size_bytes           bigint,
    checksum_sha256           char(64),

    signed_at                 timestamptz,
    captured_by_user_id       uuid,
    device_info               jsonb NOT NULL DEFAULT '{}'::jsonb,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_journal_entry_signatures_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_entry_signatures_signer
        FOREIGN KEY (journal_entry_signer_id) REFERENCES journal.journal_entry_signers (journal_entry_signer_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_journal_entry_signatures_captured_by
        FOREIGN KEY (captured_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT chk_journal_entry_signatures_size
        CHECK (file_size_bytes IS NULL OR file_size_bytes >= 0)
);

CREATE INDEX IF NOT EXISTS ix_journal_entry_signatures_tenant_id
    ON journal.journal_entry_signatures (tenant_id);

CREATE INDEX IF NOT EXISTS ix_journal_entry_signatures_signer_id
    ON journal.journal_entry_signatures (journal_entry_signer_id);

CREATE INDEX IF NOT EXISTS ix_journal_entry_signatures_signed_at
    ON journal.journal_entry_signatures (signed_at);

DROP TRIGGER IF EXISTS trg_journal_entry_signatures_updated_at ON journal.journal_entry_signatures;
CREATE TRIGGER trg_journal_entry_signatures_updated_at
BEFORE UPDATE ON journal.journal_entry_signatures
FOR EACH ROW EXECUTE FUNCTION journal.set_updated_at();

-- =========================================================
-- JOURNAL ENTRY THUMBPRINTS
-- Thumbprint capture required by some states
-- =========================================================

CREATE TABLE IF NOT EXISTS journal.journal_entry_thumbprints (
    journal_entry_thumbprint_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    journal_entry_signer_id   uuid NOT NULL,

    thumbprint_method         varchar(100),
    file_name                 varchar(255),
    mime_type                 varchar(100),
    storage_provider          varchar(50) NOT NULL DEFAULT 'object_storage',
    storage_key               text,
    file_size_bytes           bigint,
    checksum_sha256           char(64),

    captured_at               timestamptz,
    captured_by_user_id       uuid,
    device_info               jsonb NOT NULL DEFAULT '{}'::jsonb,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_journal_entry_thumbprints_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_entry_thumbprints_signer
        FOREIGN KEY (journal_entry_signer_id) REFERENCES journal.journal_entry_signers (journal_entry_signer_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_journal_entry_thumbprints_captured_by
        FOREIGN KEY (captured_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT chk_journal_entry_thumbprints_size
        CHECK (file_size_bytes IS NULL OR file_size_bytes >= 0)
);

CREATE INDEX IF NOT EXISTS ix_journal_entry_thumbprints_tenant_id
    ON journal.journal_entry_thumbprints (tenant_id);

CREATE INDEX IF NOT EXISTS ix_journal_entry_thumbprints_signer_id
    ON journal.journal_entry_thumbprints (journal_entry_signer_id);

CREATE INDEX IF NOT EXISTS ix_journal_entry_thumbprints_captured_at
    ON journal.journal_entry_thumbprints (captured_at);

DROP TRIGGER IF EXISTS trg_journal_entry_thumbprints_updated_at ON journal.journal_entry_thumbprints;
CREATE TRIGGER trg_journal_entry_thumbprints_updated_at
BEFORE UPDATE ON journal.journal_entry_thumbprints
FOR EACH ROW EXECUTE FUNCTION journal.set_updated_at();

-- =========================================================
-- JOURNAL ENTRY LINKS
-- Optional explicit link table for act/document relationships
-- =========================================================

CREATE TABLE IF NOT EXISTS journal.journal_entry_links (
    journal_entry_link_id     uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    journal_entry_id          uuid NOT NULL,

    link_type                 varchar(100) NOT NULL,
    linked_entity_type        varchar(100) NOT NULL,
    linked_entity_id          uuid NOT NULL,
    linked_entity_code        varchar(100),
    notes                     text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_journal_entry_links_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_entry_links_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS ix_journal_entry_links_tenant_id
    ON journal.journal_entry_links (tenant_id);

CREATE INDEX IF NOT EXISTS ix_journal_entry_links_entry_id
    ON journal.journal_entry_links (journal_entry_id);

CREATE INDEX IF NOT EXISTS ix_journal_entry_links_linked_entity
    ON journal.journal_entry_links (linked_entity_type, linked_entity_id);

DROP TRIGGER IF EXISTS trg_journal_entry_links_updated_at ON journal.journal_entry_links;
CREATE TRIGGER trg_journal_entry_links_updated_at
BEFORE UPDATE ON journal.journal_entry_links
FOR EACH ROW EXECUTE FUNCTION journal.set_updated_at();

-- =========================================================
-- JOURNAL AUDIT LOGS
-- Evidence-grade history for every important journal action
-- =========================================================

CREATE TABLE IF NOT EXISTS journal.journal_audit_logs (
    journal_audit_log_id      uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    journal_entry_id          uuid NOT NULL,

    event_type                journal.journal_audit_event_type NOT NULL,
    event_title               varchar(200),
    event_body                text,

    actor_user_id             uuid,
    actor_notary_id           uuid,
    source_ip                 inet,
    user_agent                text,

    before_data               jsonb,
    after_data                jsonb,
    occurred_at               timestamptz NOT NULL DEFAULT now(),
    source_reference          text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_journal_audit_logs_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_audit_logs_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_journal_audit_logs_actor_user
        FOREIGN KEY (actor_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_journal_audit_logs_actor_notary
        FOREIGN KEY (actor_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_journal_audit_logs_tenant_id
    ON journal.journal_audit_logs (tenant_id);

CREATE INDEX IF NOT EXISTS ix_journal_audit_logs_entry_id
    ON journal.journal_audit_logs (journal_entry_id);

CREATE INDEX IF NOT EXISTS ix_journal_audit_logs_event_type
    ON journal.journal_audit_logs (event_type);

CREATE INDEX IF NOT EXISTS ix_journal_audit_logs_occurred_at
    ON journal.journal_audit_logs (occurred_at);

-- =========================================================
-- JOURNAL RETENTION POLICIES
-- State-based retention rules
-- =========================================================

CREATE TABLE IF NOT EXISTS journal.journal_retention_policies (
    retention_policy_id       uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,

    policy_code               varchar(50) NOT NULL,
    policy_name               varchar(200) NOT NULL,
    state_code                char(2) NOT NULL,

    retention_years           integer NOT NULL,
    is_legal_hold_eligible    boolean NOT NULL DEFAULT true,
    export_allowed            boolean NOT NULL DEFAULT true,
    transfer_allowed          boolean NOT NULL DEFAULT true,

    effective_from            date NOT NULL,
    effective_to              date,
    is_active                 boolean NOT NULL DEFAULT true,

    notes                     text,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id        uuid,
    updated_by_user_id        uuid,
    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_journal_retention_policies_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_retention_policies_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_journal_retention_policies_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_journal_retention_policies_tenant_code
        UNIQUE (tenant_id, policy_code),

    CONSTRAINT chk_journal_retention_policies_years
        CHECK (retention_years >= 0),

    CONSTRAINT chk_journal_retention_policies_dates
        CHECK (effective_to IS NULL OR effective_to > effective_from)
);

CREATE INDEX IF NOT EXISTS ix_journal_retention_policies_tenant_id
    ON journal.journal_retention_policies (tenant_id);

CREATE INDEX IF NOT EXISTS ix_journal_retention_policies_state_code
    ON journal.journal_retention_policies (state_code);

CREATE INDEX IF NOT EXISTS ix_journal_retention_policies_is_active
    ON journal.journal_retention_policies (is_active);

DROP TRIGGER IF EXISTS trg_journal_retention_policies_updated_at ON journal.journal_retention_policies;
CREATE TRIGGER trg_journal_retention_policies_updated_at
BEFORE UPDATE ON journal.journal_retention_policies
FOR EACH ROW EXECUTE FUNCTION journal.set_updated_at();

-- =========================================================
-- JOURNAL EXPORTS
-- Export packages for audits, regulators, legal requests
-- =========================================================

CREATE TABLE IF NOT EXISTS journal.journal_exports (
    journal_export_id         uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    journal_entry_id          uuid,

    export_code               varchar(50) NOT NULL,
    export_format             journal.journal_export_format NOT NULL DEFAULT 'pdf',
    export_status             journal.journal_export_status NOT NULL DEFAULT 'queued',

    export_scope              varchar(100) NOT NULL DEFAULT 'single_entry',
    export_reason             text,
    file_name                 varchar(255),
    mime_type                 varchar(100),
    storage_provider          varchar(50) NOT NULL DEFAULT 'object_storage',
    storage_key               text,
    file_size_bytes           bigint,
    checksum_sha256           char(64),

    requested_by_user_id      uuid NOT NULL,
    generated_by_user_id      uuid,
    requested_at              timestamptz NOT NULL DEFAULT now(),
    generated_at              timestamptz,
    downloaded_at             timestamptz,
    expires_at                timestamptz,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_journal_exports_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_exports_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_journal_exports_requested_by
        FOREIGN KEY (requested_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_exports_generated_by
        FOREIGN KEY (generated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_journal_exports_tenant_code
        UNIQUE (tenant_id, export_code),

    CONSTRAINT chk_journal_exports_size
        CHECK (file_size_bytes IS NULL OR file_size_bytes >= 0)
);

CREATE INDEX IF NOT EXISTS ix_journal_exports_tenant_id
    ON journal.journal_exports (tenant_id);

CREATE INDEX IF NOT EXISTS ix_journal_exports_entry_id
    ON journal.journal_exports (journal_entry_id);

CREATE INDEX IF NOT EXISTS ix_journal_exports_status
    ON journal.journal_exports (export_status);

CREATE INDEX IF NOT EXISTS ix_journal_exports_requested_at
    ON journal.journal_exports (requested_at);

DROP TRIGGER IF EXISTS trg_journal_exports_updated_at ON journal.journal_exports;
CREATE TRIGGER trg_journal_exports_updated_at
BEFORE UPDATE ON journal.journal_exports
FOR EACH ROW EXECUTE FUNCTION journal.set_updated_at();

-- =========================================================
-- JOURNAL TRANSFER LOGS
-- Track transfer to regulator / custodian / successor notary
-- =========================================================

CREATE TABLE IF NOT EXISTS journal.journal_transfer_logs (
    journal_transfer_log_id    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                 uuid NOT NULL,
    journal_entry_id          uuid,

    transfer_code             varchar(50) NOT NULL,
    transfer_type             journal.journal_transfer_type NOT NULL,
    transfer_status           varchar(50) NOT NULL DEFAULT 'pending',

    transferred_to_name       varchar(200),
    transferred_to_entity     varchar(200),
    transferred_to_contact    varchar(200),

    transfer_reference        text,
    effective_at              timestamptz NOT NULL DEFAULT now(),
    completed_at              timestamptz,
    reason                    text,

    requested_by_user_id      uuid NOT NULL,
    approved_by_user_id       uuid,
    metadata                  jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at                timestamptz NOT NULL DEFAULT now(),
    updated_at                timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_journal_transfer_logs_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_transfer_logs_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_journal_transfer_logs_requested_by
        FOREIGN KEY (requested_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_journal_transfer_logs_approved_by
        FOREIGN KEY (approved_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_journal_transfer_logs_tenant_code
        UNIQUE (tenant_id, transfer_code)
);

CREATE INDEX IF NOT EXISTS ix_journal_transfer_logs_tenant_id
    ON journal.journal_transfer_logs (tenant_id);

CREATE INDEX IF NOT EXISTS ix_journal_transfer_logs_entry_id
    ON journal.journal_transfer_logs (journal_entry_id);

CREATE INDEX IF NOT EXISTS ix_journal_transfer_logs_transfer_type
    ON journal.journal_transfer_logs (transfer_type);

CREATE INDEX IF NOT EXISTS ix_journal_transfer_logs_effective_at
    ON journal.journal_transfer_logs (effective_at);

DROP TRIGGER IF EXISTS trg_journal_transfer_logs_updated_at ON journal.journal_transfer_logs;
CREATE TRIGGER trg_journal_transfer_logs_updated_at
BEFORE UPDATE ON journal.journal_transfer_logs
FOR EACH ROW EXECUTE FUNCTION journal.set_updated_at();

-- =========================================================
-- VIEWS
-- =========================================================

CREATE OR REPLACE VIEW journal.v_journal_entry_overview AS
SELECT
    je.journal_entry_id,
    je.tenant_id,
    je.entry_code,
    je.entry_status,
    je.act_id,
    a.act_code,
    a.act_type,
    je.notary_id,
    n.public_display_name AS notary_name,
    je.state_code,
    je.entry_date,
    je.entry_timestamp,
    je.signer_count,
    je.fee_charged,
    je.currency_code,
    je.is_complete,
    je.is_locked,
    je.completed_at,
    je.locked_at,
    je.voided_at
FROM journal.journal_entries je
JOIN notarial.notarial_acts a ON a.act_id = je.act_id
JOIN identity.notaries n ON n.notary_id = je.notary_id
WHERE je.deleted_at IS NULL
  AND a.deleted_at IS NULL
  AND n.deleted_at IS NULL;

CREATE OR REPLACE VIEW journal.v_journal_entry_signer_overview AS
SELECT
    jes.journal_entry_signer_id,
    jes.tenant_id,
    jes.journal_entry_id,
    jes.signer_order,
    jes.full_legal_name,
    jes.role_or_capacity,
    jes.signer_role,
    jes.verification_method,
    jes.verification_result,
    jes.is_primary_signer,
    jes.is_missing_signature,
    jes.is_missing_thumbprint
FROM journal.journal_entry_signers jes
WHERE jes.deleted_at IS NULL;

CREATE OR REPLACE VIEW journal.v_journal_compliance_summary AS
SELECT
    je.tenant_id,
    je.state_code,
    COUNT(*) AS total_entries,
    COUNT(*) FILTER (WHERE je.entry_status = 'locked') AS locked_entries,
    COUNT(*) FILTER (WHERE je.is_missing_signature = true) AS missing_signature_entries,
    COUNT(*) FILTER (WHERE je.is_missing_thumbprint = true) AS missing_thumbprint_entries,
    COUNT(*) FILTER (WHERE je.voided_at IS NOT NULL) AS voided_entries
FROM journal.journal_entries je
WHERE je.deleted_at IS NULL
GROUP BY je.tenant_id, je.state_code;

-- =========================================================
-- OPTIONAL POST-CREATION LINK BACK TO NOTARIAL
-- If notarial schema already exists, you can enable the FK below.
-- =========================================================

DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM information_schema.tables
        WHERE table_schema = 'notarial'
          AND table_name = 'notarial_acts'
    ) THEN
        -- Add FK from notarial.notarial_acts -> journal.journal_entries if not present
        IF NOT EXISTS (
            SELECT 1
            FROM pg_constraint
            WHERE conname = 'fk_notarial_acts_linked_journal_entry'
        ) THEN
            ALTER TABLE notarial.notarial_acts
                ADD CONSTRAINT fk_notarial_acts_linked_journal_entry
                FOREIGN KEY (linked_journal_entry_id)
                REFERENCES journal.journal_entries (journal_entry_id)
                ON DELETE SET NULL;
        END IF;
    END IF;
END $$;

-- =========================================================
-- COMMENTS
-- =========================================================

COMMENT ON SCHEMA journal IS 'Journal schema for immutable legal journal entries, signer records, captures, audit, retention, export, and transfer.';
COMMENT ON TABLE journal.journal_entries IS 'Central journal entry for each notarial act; locked entries become immutable.';
COMMENT ON TABLE journal.journal_entry_signers IS 'Signer and identity information for each journal entry.';
COMMENT ON TABLE journal.journal_entry_id_documents IS 'ID scans and identity evidence for journal signers.';
COMMENT ON TABLE journal.journal_entry_signatures IS 'Signature capture records for journal signers.';
COMMENT ON TABLE journal.journal_entry_thumbprints IS 'Thumbprint capture records for journal signers.';
COMMENT ON TABLE journal.journal_entry_links IS 'Optional generic links to related entities.';
COMMENT ON TABLE journal.journal_audit_logs IS 'Evidence-grade audit trail for journal operations.';
COMMENT ON TABLE journal.journal_retention_policies IS 'State-based retention rules for journal entries.';
COMMENT ON TABLE journal.journal_exports IS 'Export requests and generated evidence packages.';
COMMENT ON TABLE journal.journal_transfer_logs IS 'Transfer logs for journal custody and regulator handoff.';


-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================

-- PostgreSQL 15+
-- Security schema for seals, digital certificates, access policies,
-- usage logs, revocations, replacements, incidents, device trust, MFA, and audit logs.

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

CREATE SCHEMA IF NOT EXISTS security;

-- =========================================================
-- ENUM TYPES
-- =========================================================

DO $$
BEGIN
    IF to_regtype('security.seal_type') IS NULL THEN
        CREATE TYPE security.seal_type AS ENUM (
            'physical_seal',
            'electronic_seal',
            'embosser',
            'stamp'
        );
    END IF;

    IF to_regtype('security.seal_status') IS NULL THEN
        CREATE TYPE security.seal_status AS ENUM (
            'active',
            'suspended',
            'revoked',
            'expired',
            'lost',
            'stolen',
            'replaced',
            'archived'
        );
    END IF;

    IF to_regtype('security.certificate_status') IS NULL THEN
        CREATE TYPE security.certificate_status AS ENUM (
            'pending',
            'active',
            'expiring',
            'expired',
            'suspended',
            'revoked',
            'replaced',
            'archived'
        );
    END IF;

    IF to_regtype('security.usage_action_type') IS NULL THEN
        CREATE TYPE security.usage_action_type AS ENUM (
            'apply_seal',
            'apply_certificate',
            'verify_signature',
            'generate_seal_artifact',
            'sign_document',
            'validate_usage'
        );
    END IF;

    IF to_regtype('security.usage_result_type') IS NULL THEN
        CREATE TYPE security.usage_result_type AS ENUM (
            'allowed',
            'denied',
            'pending_approval',
            'failed'
        );
    END IF;

    IF to_regtype('security.policy_status') IS NULL THEN
        CREATE TYPE security.policy_status AS ENUM (
            'draft',
            'active',
            'inactive',
            'archived'
        );
    END IF;

    IF to_regtype('security.policy_scope') IS NULL THEN
        CREATE TYPE security.policy_scope AS ENUM (
            'tenant',
            'branch',
            'region',
            'state',
            'notary',
            'role'
        );
    END IF;

    IF to_regtype('security.policy_target_type') IS NULL THEN
        CREATE TYPE security.policy_target_type AS ENUM (
            'seal',
            'certificate',
            'both'
        );
    END IF;

    IF to_regtype('security.incident_type') IS NULL THEN
        CREATE TYPE security.incident_type AS ENUM (
            'lost_seal',
            'stolen_seal',
            'compromised_private_key',
            'unauthorized_use',
            'expired_asset_use',
            'suspicious_activity',
            'access_violation',
            'data_breach',
            'other'
        );
    END IF;

    IF to_regtype('security.incident_severity') IS NULL THEN
        CREATE TYPE security.incident_severity AS ENUM (
            'low',
            'medium',
            'high',
            'critical'
        );
    END IF;

    IF to_regtype('security.incident_status') IS NULL THEN
        CREATE TYPE security.incident_status AS ENUM (
            'open',
            'investigating',
            'contained',
            'resolved',
            'closed',
            'escalated'
        );
    END IF;

    IF to_regtype('security.lock_status') IS NULL THEN
        CREATE TYPE security.lock_status AS ENUM (
            'active',
            'released',
            'expired',
            'cancelled'
        );
    END IF;

    IF to_regtype('security.device_status') IS NULL THEN
        CREATE TYPE security.device_status AS ENUM (
            'pending',
            'trusted',
            'revoked',
            'expired'
        );
    END IF;

    IF to_regtype('security.mfa_method_type') IS NULL THEN
        CREATE TYPE security.mfa_method_type AS ENUM (
            'totp',
            'sms',
            'email',
            'push',
            'hardware_key',
            'recovery_code'
        );
    END IF;

    IF to_regtype('security.audit_event_type') IS NULL THEN
        CREATE TYPE security.audit_event_type AS ENUM (
            'create',
            'update',
            'status_change',
            'activate',
            'suspend',
            'revoke',
            'replace',
            'lock',
            'unlock',
            'usage_allowed',
            'usage_denied',
            'incident_opened',
            'incident_updated',
            'incident_closed',
            'policy_changed',
            'device_trusted',
            'device_revoked',
            'mfa_added',
            'mfa_removed'
        );
    END IF;

    IF to_regtype('security.replacement_reason_type') IS NULL THEN
        CREATE TYPE security.replacement_reason_type AS ENUM (
            'lost',
            'stolen',
            'compromised',
            'expired',
            'damaged',
            'renewal',
            'other'
        );
    END IF;
END $$;

-- =========================================================
-- UPDATED_AT TRIGGER
-- =========================================================

CREATE OR REPLACE FUNCTION security.set_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at = now();
    RETURN NEW;
END;
$$;

-- =========================================================
-- SEALS
-- =========================================================

CREATE TABLE IF NOT EXISTS security.seals (
    seal_id                 uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    seal_code               varchar(50)  NOT NULL,
    seal_name               varchar(200) NOT NULL,
    seal_type               security.seal_type NOT NULL DEFAULT 'physical_seal',
    status                  security.seal_status NOT NULL DEFAULT 'active',

    notary_id               uuid NOT NULL,
    state_code              char(2) NOT NULL,
    commission_number       varchar(100),
    seal_number             varchar(100),
    issuing_authority       varchar(200),

    effective_from          date,
    expires_on              date,
    issued_at               timestamptz,
    suspended_at            timestamptz,
    revoked_at              timestamptz,
    lost_at                 timestamptz,
    stolen_at               timestamptz,
    replaced_at             timestamptz,

    physical_description    text,
    image_file_name         varchar(255),
    image_mime_type         varchar(100),
    image_storage_provider  varchar(50) NOT NULL DEFAULT 'object_storage',
    image_storage_key       text,
    image_checksum_sha256   char(64),

    last_used_at            timestamptz,
    usage_count             integer NOT NULL DEFAULT 0,

    is_centralized          boolean NOT NULL DEFAULT true,
    is_available_for_use    boolean NOT NULL DEFAULT true,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_seals_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_seals_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_seals_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_seals_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_seals_tenant_code
        UNIQUE (tenant_id, seal_code),

    CONSTRAINT uq_seals_tenant_number
        UNIQUE (tenant_id, seal_number),

    CONSTRAINT chk_seals_usage_count
        CHECK (usage_count >= 0),

    CONSTRAINT chk_seals_date_order
        CHECK (expires_on IS NULL OR effective_from IS NULL OR expires_on >= effective_from)
);

CREATE INDEX IF NOT EXISTS ix_seals_tenant_id
    ON security.seals (tenant_id);

CREATE INDEX IF NOT EXISTS ix_seals_notary_id
    ON security.seals (notary_id);

CREATE INDEX IF NOT EXISTS ix_seals_status
    ON security.seals (status);

CREATE INDEX IF NOT EXISTS ix_seals_seal_type
    ON security.seals (seal_type);

CREATE INDEX IF NOT EXISTS ix_seals_state_code
    ON security.seals (state_code);

CREATE INDEX IF NOT EXISTS ix_seals_expires_on
    ON security.seals (expires_on);

DROP TRIGGER IF EXISTS trg_seals_updated_at ON security.seals;
CREATE TRIGGER trg_seals_updated_at
BEFORE UPDATE ON security.seals
FOR EACH ROW EXECUTE FUNCTION security.set_updated_at();

-- =========================================================
-- DIGITAL CERTIFICATES
-- =========================================================

CREATE TABLE IF NOT EXISTS security.digital_certificates (
    digital_certificate_id   uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    certificate_code         varchar(50)  NOT NULL,
    certificate_name         varchar(200) NOT NULL,
    status                   security.certificate_status NOT NULL DEFAULT 'pending',

    notary_id                uuid NOT NULL,
    seal_id                  uuid,

    provider_name            varchar(200) NOT NULL,
    subject_common_name      varchar(200),
    issuer_common_name       varchar(200),
    serial_number            varchar(150) NOT NULL,
    thumbprint_sha1          char(40),
    thumbprint_sha256        char(64),

    cryptographic_algorithm  varchar(100) NOT NULL,
    key_storage_method       varchar(100) NOT NULL, -- HSM / secure vault / token / other
    hsm_label                varchar(100),
    key_rotation_status      varchar(100) NOT NULL DEFAULT 'not_rotated',

    valid_from               timestamptz,
    valid_to                 timestamptz,
    activated_at             timestamptz,
    suspended_at             timestamptz,
    revoked_at               timestamptz,
    replaced_at              timestamptz,

    revocation_reason        text,
    certificate_chain        jsonb NOT NULL DEFAULT '[]'::jsonb,
    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id       uuid,
    updated_by_user_id       uuid,
    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_digital_certificates_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_digital_certificates_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_digital_certificates_seal
        FOREIGN KEY (seal_id) REFERENCES security.seals (seal_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_digital_certificates_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_digital_certificates_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_digital_certificates_tenant_code
        UNIQUE (tenant_id, certificate_code),

    CONSTRAINT uq_digital_certificates_serial_number
        UNIQUE (tenant_id, serial_number)
);

CREATE INDEX IF NOT EXISTS ix_digital_certificates_tenant_id
    ON security.digital_certificates (tenant_id);

CREATE INDEX IF NOT EXISTS ix_digital_certificates_notary_id
    ON security.digital_certificates (notary_id);

CREATE INDEX IF NOT EXISTS ix_digital_certificates_seal_id
    ON security.digital_certificates (seal_id);

CREATE INDEX IF NOT EXISTS ix_digital_certificates_status
    ON security.digital_certificates (status);

CREATE INDEX IF NOT EXISTS ix_digital_certificates_valid_to
    ON security.digital_certificates (valid_to);

DROP TRIGGER IF EXISTS trg_digital_certificates_updated_at ON security.digital_certificates;
CREATE TRIGGER trg_digital_certificates_updated_at
BEFORE UPDATE ON security.digital_certificates
FOR EACH ROW EXECUTE FUNCTION security.set_updated_at();

-- =========================================================
-- CERTIFICATE CHAIN ITEMS
-- =========================================================

CREATE TABLE IF NOT EXISTS security.digital_certificate_chain_items (
    chain_item_id            uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    digital_certificate_id    uuid NOT NULL,

    chain_order              integer NOT NULL,
    subject_name             varchar(200),
    issuer_name              varchar(200),
    serial_number            varchar(150),
    thumbprint_sha256        char(64),
    valid_from               timestamptz,
    valid_to                 timestamptz,
    is_root                  boolean NOT NULL DEFAULT false,
    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_chain_items_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_chain_items_certificate
        FOREIGN KEY (digital_certificate_id) REFERENCES security.digital_certificates (digital_certificate_id)
        ON DELETE CASCADE,

    CONSTRAINT chk_chain_items_order
        CHECK (chain_order >= 1)
);

CREATE INDEX IF NOT EXISTS ix_chain_items_tenant_id
    ON security.digital_certificate_chain_items (tenant_id);

CREATE INDEX IF NOT EXISTS ix_chain_items_certificate_id
    ON security.digital_certificate_chain_items (digital_certificate_id);

CREATE INDEX IF NOT EXISTS ix_chain_items_chain_order
    ON security.digital_certificate_chain_items (chain_order);

DROP TRIGGER IF EXISTS trg_chain_items_updated_at ON security.digital_certificate_chain_items;
CREATE TRIGGER trg_chain_items_updated_at
BEFORE UPDATE ON security.digital_certificate_chain_items
FOR EACH ROW EXECUTE FUNCTION security.set_updated_at();

-- =========================================================
-- ACCESS POLICIES
-- =========================================================

CREATE TABLE IF NOT EXISTS security.seal_access_policies (
    policy_id               uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    policy_code             varchar(50)  NOT NULL,
    policy_name             varchar(200) NOT NULL,
    status                  security.policy_status NOT NULL DEFAULT 'draft',

    target_type             security.policy_target_type NOT NULL DEFAULT 'both',
    scope                   security.policy_scope NOT NULL DEFAULT 'tenant',

    state_code              char(2),
    branch_id               uuid,
    region_id               uuid,
    notary_id               uuid,
    service_type_id         uuid,

    required_roles          jsonb NOT NULL DEFAULT '[]'::jsonb,
    required_permissions    jsonb NOT NULL DEFAULT '[]'::jsonb,
    conditions              jsonb NOT NULL DEFAULT '{}'::jsonb,
    approval_workflow       jsonb NOT NULL DEFAULT '{}'::jsonb,

    mfa_required            boolean NOT NULL DEFAULT true,
    approval_required       boolean NOT NULL DEFAULT false,
    delegation_allowed      boolean NOT NULL DEFAULT false,
    emergency_override_allowed boolean NOT NULL DEFAULT false,

    effective_from          timestamptz,
    effective_to            timestamptz,
    notes                   text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_policies_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_policies_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_policies_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_policies_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_policies_service_type
        FOREIGN KEY (service_type_id) REFERENCES operations.service_types (service_type_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_policies_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_policies_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_policies_tenant_code
        UNIQUE (tenant_id, policy_code),

    CONSTRAINT chk_policies_dates
        CHECK (effective_to IS NULL OR effective_from IS NULL OR effective_to >= effective_from)
);

CREATE INDEX IF NOT EXISTS ix_policies_tenant_id
    ON security.seal_access_policies (tenant_id);

CREATE INDEX IF NOT EXISTS ix_policies_status
    ON security.seal_access_policies (status);

CREATE INDEX IF NOT EXISTS ix_policies_scope
    ON security.seal_access_policies (scope);

CREATE INDEX IF NOT EXISTS ix_policies_state_code
    ON security.seal_access_policies (state_code);

CREATE INDEX IF NOT EXISTS ix_policies_branch_id
    ON security.seal_access_policies (branch_id);

CREATE INDEX IF NOT EXISTS ix_policies_region_id
    ON security.seal_access_policies (region_id);

DROP TRIGGER IF EXISTS trg_policies_updated_at ON security.seal_access_policies;
CREATE TRIGGER trg_policies_updated_at
BEFORE UPDATE ON security.seal_access_policies
FOR EACH ROW EXECUTE FUNCTION security.set_updated_at();

-- =========================================================
-- DEVICE TRUST
-- =========================================================

CREATE TABLE IF NOT EXISTS security.trusted_devices (
    trusted_device_id        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    user_id                  uuid NOT NULL,

    device_code              varchar(50) NOT NULL,
    device_name              varchar(200),
    device_fingerprint       text NOT NULL,
    platform                 varchar(100),
    browser                  varchar(100),
    os_name                  varchar(100),
    ip_address               inet,
    first_seen_at            timestamptz NOT NULL DEFAULT now(),
    last_seen_at             timestamptz,
    trusted_at               timestamptz,
    revoked_at               timestamptz,
    status                   security.device_status NOT NULL DEFAULT 'pending',

    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_trusted_devices_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_trusted_devices_user
        FOREIGN KEY (user_id) REFERENCES core.users (user_id)
        ON DELETE CASCADE,

    CONSTRAINT uq_trusted_devices_tenant_code
        UNIQUE (tenant_id, device_code),

    CONSTRAINT uq_trusted_devices_fingerprint
        UNIQUE (tenant_id, device_fingerprint)
);

CREATE INDEX IF NOT EXISTS ix_trusted_devices_tenant_id
    ON security.trusted_devices (tenant_id);

CREATE INDEX IF NOT EXISTS ix_trusted_devices_user_id
    ON security.trusted_devices (user_id);

CREATE INDEX IF NOT EXISTS ix_trusted_devices_status
    ON security.trusted_devices (status);

DROP TRIGGER IF EXISTS trg_trusted_devices_updated_at ON security.trusted_devices;
CREATE TRIGGER trg_trusted_devices_updated_at
BEFORE UPDATE ON security.trusted_devices
FOR EACH ROW EXECUTE FUNCTION security.set_updated_at();

-- =========================================================
-- MFA DEVICES
-- =========================================================

CREATE TABLE IF NOT EXISTS security.mfa_devices (
    mfa_device_id            uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    user_id                  uuid NOT NULL,

    mfa_device_code          varchar(50) NOT NULL,
    method_type              security.mfa_method_type NOT NULL,
    label                    varchar(200),
    secret_reference         text,
    phone_or_email           citext,
    is_primary               boolean NOT NULL DEFAULT false,
    is_verified              boolean NOT NULL DEFAULT false,
    verified_at              timestamptz,
    last_used_at             timestamptz,
    revoked_at               timestamptz,
    status                   security.device_status NOT NULL DEFAULT 'pending',

    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at                timestamptz,

    CONSTRAINT fk_mfa_devices_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_mfa_devices_user
        FOREIGN KEY (user_id) REFERENCES core.users (user_id)
        ON DELETE CASCADE,

    CONSTRAINT uq_mfa_devices_tenant_code
        UNIQUE (tenant_id, mfa_device_code)
);

CREATE INDEX IF NOT EXISTS ix_mfa_devices_tenant_id
    ON security.mfa_devices (tenant_id);

CREATE INDEX IF NOT EXISTS ix_mfa_devices_user_id
    ON security.mfa_devices (user_id);

CREATE INDEX IF NOT EXISTS ix_mfa_devices_status
    ON security.mfa_devices (status);

CREATE INDEX IF NOT EXISTS ix_mfa_devices_method_type
    ON security.mfa_devices (method_type);

DROP TRIGGER IF EXISTS trg_mfa_devices_updated_at ON security.mfa_devices;
CREATE TRIGGER trg_mfa_devices_updated_at
BEFORE UPDATE ON security.mfa_devices
FOR EACH ROW EXECUTE FUNCTION security.set_updated_at();

CREATE UNIQUE INDEX IF NOT EXISTS ux_mfa_devices_one_primary
    ON security.mfa_devices (user_id)
    WHERE is_primary = true AND deleted_at IS NULL;

-- =========================================================
-- INCIDENTS
-- =========================================================

CREATE TABLE IF NOT EXISTS security.security_incidents (
    incident_id              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    incident_code            varchar(50) NOT NULL,
    incident_type            security.incident_type NOT NULL,
    severity                 security.incident_severity NOT NULL DEFAULT 'medium',
    status                   security.incident_status NOT NULL DEFAULT 'open',

    title                    varchar(250) NOT NULL,
    summary                  text,
    details                  text,

    detected_at              timestamptz NOT NULL DEFAULT now(),
    reported_at              timestamptz,
    contained_at             timestamptz,
    resolved_at              timestamptz,
    closed_at                timestamptz,

    reported_by_user_id      uuid,
    assigned_to_user_id      uuid,
    primary_notary_id        uuid,
    affected_seal_id         uuid,
    affected_certificate_id  uuid,
    affected_device_id       uuid,

    legal_hold               boolean NOT NULL DEFAULT false,
    regulatory_notification_required boolean NOT NULL DEFAULT false,
    regulatory_notified_at   timestamptz,
    external_reference       text,

    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_security_incidents_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_security_incidents_reported_by
        FOREIGN KEY (reported_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_security_incidents_assigned_to
        FOREIGN KEY (assigned_to_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_security_incidents_primary_notary
        FOREIGN KEY (primary_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_security_incidents_affected_seal
        FOREIGN KEY (affected_seal_id) REFERENCES security.seals (seal_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_security_incidents_affected_certificate
        FOREIGN KEY (affected_certificate_id) REFERENCES security.digital_certificates (digital_certificate_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_security_incidents_affected_device
        FOREIGN KEY (affected_device_id) REFERENCES security.trusted_devices (trusted_device_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_security_incidents_tenant_code
        UNIQUE (tenant_id, incident_code)
);

CREATE INDEX IF NOT EXISTS ix_security_incidents_tenant_id
    ON security.security_incidents (tenant_id);

CREATE INDEX IF NOT EXISTS ix_security_incidents_status
    ON security.security_incidents (status);

CREATE INDEX IF NOT EXISTS ix_security_incidents_severity
    ON security.security_incidents (severity);

CREATE INDEX IF NOT EXISTS ix_security_incidents_incident_type
    ON security.security_incidents (incident_type);

CREATE INDEX IF NOT EXISTS ix_security_incidents_detected_at
    ON security.security_incidents (detected_at);

DROP TRIGGER IF EXISTS trg_security_incidents_updated_at ON security.security_incidents;
CREATE TRIGGER trg_security_incidents_updated_at
BEFORE UPDATE ON security.security_incidents
FOR EACH ROW EXECUTE FUNCTION security.set_updated_at();

-- =========================================================
-- INCIDENT ACTIONS
-- =========================================================

CREATE TABLE IF NOT EXISTS security.security_incident_actions (
    incident_action_id       uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    incident_id              uuid NOT NULL,

    action_code              varchar(50) NOT NULL,
    action_title             varchar(200) NOT NULL,
    action_body              text,

    action_status            varchar(50) NOT NULL DEFAULT 'open',
    performed_by_user_id     uuid,
    performed_at             timestamptz NOT NULL DEFAULT now(),
    due_at                   timestamptz,
    completed_at             timestamptz,

    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_incident_actions_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_incident_actions_incident
        FOREIGN KEY (incident_id) REFERENCES security.security_incidents (incident_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_incident_actions_performed_by
        FOREIGN KEY (performed_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_incident_actions_incident_code
        UNIQUE (incident_id, action_code)
);

CREATE INDEX IF NOT EXISTS ix_incident_actions_tenant_id
    ON security.security_incident_actions (tenant_id);

CREATE INDEX IF NOT EXISTS ix_incident_actions_incident_id
    ON security.security_incident_actions (incident_id);

CREATE INDEX IF NOT EXISTS ix_incident_actions_performed_at
    ON security.security_incident_actions (performed_at);

DROP TRIGGER IF EXISTS trg_incident_actions_updated_at ON security.security_incident_actions;
CREATE TRIGGER trg_incident_actions_updated_at
BEFORE UPDATE ON security.security_incident_actions
FOR EACH ROW EXECUTE FUNCTION security.set_updated_at();

-- =========================================================
-- REVOCATIONS
-- =========================================================

CREATE TABLE IF NOT EXISTS security.seal_revocations (
    revocation_id            uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    revocation_code          varchar(50) NOT NULL,

    seal_id                  uuid,
    digital_certificate_id   uuid,
    incident_id              uuid,

    revoked_by_user_id       uuid NOT NULL,
    revoked_at               timestamptz NOT NULL DEFAULT now(),
    effective_at             timestamptz NOT NULL DEFAULT now(),

    reason_type              security.replacement_reason_type NOT NULL DEFAULT 'other',
    reason                   text NOT NULL,
    regulatory_notification_required boolean NOT NULL DEFAULT false,
    regulatory_notified_at   timestamptz,
    regulatory_reference     text,

    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_revocations_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_revocations_seal
        FOREIGN KEY (seal_id) REFERENCES security.seals (seal_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_revocations_certificate
        FOREIGN KEY (digital_certificate_id) REFERENCES security.digital_certificates (digital_certificate_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_revocations_incident
        FOREIGN KEY (incident_id) REFERENCES security.security_incidents (incident_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_revocations_revoked_by
        FOREIGN KEY (revoked_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT uq_revocations_tenant_code
        UNIQUE (tenant_id, revocation_code),

    CONSTRAINT chk_revocations_target_present
        CHECK (seal_id IS NOT NULL OR digital_certificate_id IS NOT NULL)
);

CREATE INDEX IF NOT EXISTS ix_revocations_tenant_id
    ON security.seal_revocations (tenant_id);

CREATE INDEX IF NOT EXISTS ix_revocations_seal_id
    ON security.seal_revocations (seal_id);

CREATE INDEX IF NOT EXISTS ix_revocations_certificate_id
    ON security.seal_revocations (digital_certificate_id);

CREATE INDEX IF NOT EXISTS ix_revocations_incident_id
    ON security.seal_revocations (incident_id);

CREATE INDEX IF NOT EXISTS ix_revocations_revoked_at
    ON security.seal_revocations (revoked_at);

DROP TRIGGER IF EXISTS trg_revocations_updated_at ON security.seal_revocations;
CREATE TRIGGER trg_revocations_updated_at
BEFORE UPDATE ON security.seal_revocations
FOR EACH ROW EXECUTE FUNCTION security.set_updated_at();

-- =========================================================
-- REPLACEMENTS
-- =========================================================

CREATE TABLE IF NOT EXISTS security.seal_replacements (
    replacement_id           uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    replacement_code         varchar(50) NOT NULL,

    old_seal_id              uuid,
    old_certificate_id       uuid,
    new_seal_id              uuid,
    new_certificate_id       uuid,

    incident_id              uuid,
    revocation_id            uuid,
    reason_type              security.replacement_reason_type NOT NULL DEFAULT 'other',
    reason                   text NOT NULL,

    issued_by_user_id        uuid NOT NULL,
    issued_at                timestamptz NOT NULL DEFAULT now(),
    effective_at             timestamptz NOT NULL DEFAULT now(),

    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_replacements_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_replacements_old_seal
        FOREIGN KEY (old_seal_id) REFERENCES security.seals (seal_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_replacements_old_certificate
        FOREIGN KEY (old_certificate_id) REFERENCES security.digital_certificates (digital_certificate_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_replacements_new_seal
        FOREIGN KEY (new_seal_id) REFERENCES security.seals (seal_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_replacements_new_certificate
        FOREIGN KEY (new_certificate_id) REFERENCES security.digital_certificates (digital_certificate_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_replacements_incident
        FOREIGN KEY (incident_id) REFERENCES security.security_incidents (incident_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_replacements_revocation
        FOREIGN KEY (revocation_id) REFERENCES security.seal_revocations (revocation_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_replacements_issued_by
        FOREIGN KEY (issued_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT uq_replacements_tenant_code
        UNIQUE (tenant_id, replacement_code),

    CONSTRAINT chk_replacements_target_present
        CHECK (
            old_seal_id IS NOT NULL
            OR old_certificate_id IS NOT NULL
            OR new_seal_id IS NOT NULL
            OR new_certificate_id IS NOT NULL
        )
);

CREATE INDEX IF NOT EXISTS ix_replacements_tenant_id
    ON security.seal_replacements (tenant_id);

CREATE INDEX IF NOT EXISTS ix_replacements_old_seal_id
    ON security.seal_replacements (old_seal_id);

CREATE INDEX IF NOT EXISTS ix_replacements_old_certificate_id
    ON security.seal_replacements (old_certificate_id);

CREATE INDEX IF NOT EXISTS ix_replacements_new_seal_id
    ON security.seal_replacements (new_seal_id);

CREATE INDEX IF NOT EXISTS ix_replacements_new_certificate_id
    ON security.seal_replacements (new_certificate_id);

CREATE INDEX IF NOT EXISTS ix_replacements_issued_at
    ON security.seal_replacements (issued_at);

DROP TRIGGER IF EXISTS trg_replacements_updated_at ON security.seal_replacements;
CREATE TRIGGER trg_replacements_updated_at
BEFORE UPDATE ON security.seal_replacements
FOR EACH ROW EXECUTE FUNCTION security.set_updated_at();

-- =========================================================
-- EMERGENCY LOCKS
-- =========================================================

CREATE TABLE IF NOT EXISTS security.emergency_locks (
    emergency_lock_id        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    lock_code                varchar(50) NOT NULL,
    lock_status              security.lock_status NOT NULL DEFAULT 'active',

    seal_id                  uuid,
    digital_certificate_id   uuid,
    incident_id              uuid,

    locked_by_user_id        uuid NOT NULL,
    locked_at                timestamptz NOT NULL DEFAULT now(),
    released_by_user_id      uuid,
    released_at              timestamptz,

    lock_reason              text NOT NULL,
    unlock_reason            text,
    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_emergency_locks_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_emergency_locks_seal
        FOREIGN KEY (seal_id) REFERENCES security.seals (seal_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_emergency_locks_certificate
        FOREIGN KEY (digital_certificate_id) REFERENCES security.digital_certificates (digital_certificate_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_emergency_locks_incident
        FOREIGN KEY (incident_id) REFERENCES security.security_incidents (incident_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_emergency_locks_locked_by
        FOREIGN KEY (locked_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_emergency_locks_released_by
        FOREIGN KEY (released_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_emergency_locks_tenant_code
        UNIQUE (tenant_id, lock_code),

    CONSTRAINT chk_emergency_locks_target_present
        CHECK (seal_id IS NOT NULL OR digital_certificate_id IS NOT NULL)
);

CREATE INDEX IF NOT EXISTS ix_emergency_locks_tenant_id
    ON security.emergency_locks (tenant_id);

CREATE INDEX IF NOT EXISTS ix_emergency_locks_status
    ON security.emergency_locks (lock_status);

CREATE INDEX IF NOT EXISTS ix_emergency_locks_seal_id
    ON security.emergency_locks (seal_id);

CREATE INDEX IF NOT EXISTS ix_emergency_locks_certificate_id
    ON security.emergency_locks (digital_certificate_id);

DROP TRIGGER IF EXISTS trg_emergency_locks_updated_at ON security.emergency_locks;
CREATE TRIGGER trg_emergency_locks_updated_at
BEFORE UPDATE ON security.emergency_locks
FOR EACH ROW EXECUTE FUNCTION security.set_updated_at();

-- =========================================================
-- USAGE LOGS
-- =========================================================

CREATE TABLE IF NOT EXISTS security.seal_usage_logs (
    usage_log_id             uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    usage_code               varchar(50) NOT NULL,

    seal_id                  uuid,
    digital_certificate_id   uuid,
    policy_id                uuid,
    incident_id              uuid,

    notarial_act_id          uuid,
    journal_entry_id         uuid,

    used_by_user_id          uuid NOT NULL,
    notary_id                uuid,
    action_type              security.usage_action_type NOT NULL,
    result                   security.usage_result_type NOT NULL DEFAULT 'allowed',

    used_at                  timestamptz NOT NULL DEFAULT now(),
    state_code               char(2),
    source_ip                inet,
    user_agent               text,
    device_id                uuid,
    reason                   text,
    denial_reason            text,
    evidence                 jsonb NOT NULL DEFAULT '{}'::jsonb,
    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at               timestamptz NOT NULL DEFAULT now(),
    updated_at               timestamptz NOT NULL DEFAULT now(),
    deleted_at               timestamptz,

    CONSTRAINT fk_usage_logs_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_usage_logs_seal
        FOREIGN KEY (seal_id) REFERENCES security.seals (seal_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_usage_logs_certificate
        FOREIGN KEY (digital_certificate_id) REFERENCES security.digital_certificates (digital_certificate_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_usage_logs_policy
        FOREIGN KEY (policy_id) REFERENCES security.seal_access_policies (policy_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_usage_logs_incident
        FOREIGN KEY (incident_id) REFERENCES security.security_incidents (incident_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_usage_logs_used_by
        FOREIGN KEY (used_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_usage_logs_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_usage_logs_notarial_act
        FOREIGN KEY (notarial_act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_usage_logs_journal_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_usage_logs_device
        FOREIGN KEY (device_id) REFERENCES security.trusted_devices (trusted_device_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_usage_logs_tenant_code
        UNIQUE (tenant_id, usage_code),

    CONSTRAINT chk_usage_logs_target_present
        CHECK (seal_id IS NOT NULL OR digital_certificate_id IS NOT NULL)
);

CREATE INDEX IF NOT EXISTS ix_usage_logs_tenant_id
    ON security.seal_usage_logs (tenant_id);

CREATE INDEX IF NOT EXISTS ix_usage_logs_seal_id
    ON security.seal_usage_logs (seal_id);

CREATE INDEX IF NOT EXISTS ix_usage_logs_certificate_id
    ON security.seal_usage_logs (digital_certificate_id);

CREATE INDEX IF NOT EXISTS ix_usage_logs_action_type
    ON security.seal_usage_logs (action_type);

CREATE INDEX IF NOT EXISTS ix_usage_logs_result
    ON security.seal_usage_logs (result);

CREATE INDEX IF NOT EXISTS ix_usage_logs_used_at
    ON security.seal_usage_logs (used_at);

CREATE INDEX IF NOT EXISTS ix_usage_logs_notarial_act_id
    ON security.seal_usage_logs (notarial_act_id);

CREATE INDEX IF NOT EXISTS ix_usage_logs_journal_entry_id
    ON security.seal_usage_logs (journal_entry_id);

DROP TRIGGER IF EXISTS trg_usage_logs_updated_at ON security.seal_usage_logs;
CREATE TRIGGER trg_usage_logs_updated_at
BEFORE UPDATE ON security.seal_usage_logs
FOR EACH ROW EXECUTE FUNCTION security.set_updated_at();

-- Enforce active assets only when result = 'allowed'
CREATE OR REPLACE FUNCTION security.validate_usage_asset_state()
RETURNS trigger
LANGUAGE plpgsql
AS $$
DECLARE
    v_seal_status security.seal_status;
    v_cert_status security.certificate_status;
BEGIN
    IF NEW.result <> 'allowed' THEN
        RETURN NEW;
    END IF;

    IF NEW.seal_id IS NOT NULL THEN
        SELECT s.status
        INTO v_seal_status
        FROM security.seals s
        WHERE s.seal_id = NEW.seal_id
          AND s.deleted_at IS NULL;

        IF v_seal_status IS NULL OR v_seal_status <> 'active' THEN
            RAISE EXCEPTION 'Seal % is not active and cannot be used.', NEW.seal_id;
        END IF;
    END IF;

    IF NEW.digital_certificate_id IS NOT NULL THEN
        SELECT dc.status
        INTO v_cert_status
        FROM security.digital_certificates dc
        WHERE dc.digital_certificate_id = NEW.digital_certificate_id
          AND dc.deleted_at IS NULL;

        IF v_cert_status IS NULL OR v_cert_status NOT IN ('active', 'expiring') THEN
            RAISE EXCEPTION 'Digital certificate % is not usable.', NEW.digital_certificate_id;
        END IF;
    END IF;

    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS trg_usage_logs_validate_asset_state ON security.seal_usage_logs;
CREATE TRIGGER trg_usage_logs_validate_asset_state
BEFORE INSERT OR UPDATE ON security.seal_usage_logs
FOR EACH ROW EXECUTE FUNCTION security.validate_usage_asset_state();

-- =========================================================
-- GENERIC SECURITY AUDIT LOGS
-- =========================================================

CREATE TABLE IF NOT EXISTS security.security_audit_logs (
    security_audit_log_id    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id                uuid NOT NULL,
    audit_code               varchar(50) NOT NULL,

    event_type               security.audit_event_type NOT NULL,
    entity_type              varchar(100) NOT NULL,
    entity_id                uuid,
    entity_code              varchar(100),

    actor_user_id            uuid,
    actor_notary_id          uuid,
    source_ip                inet,
    user_agent               text,

    before_data              jsonb,
    after_data               jsonb,
    occurred_at              timestamptz NOT NULL DEFAULT now(),
    source_reference         text,
    metadata                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at               timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_security_audit_logs_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_security_audit_logs_actor_user
        FOREIGN KEY (actor_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_security_audit_logs_actor_notary
        FOREIGN KEY (actor_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_security_audit_logs_tenant_code
        UNIQUE (tenant_id, audit_code)
);

CREATE INDEX IF NOT EXISTS ix_security_audit_logs_tenant_id
    ON security.security_audit_logs (tenant_id);

CREATE INDEX IF NOT EXISTS ix_security_audit_logs_event_type
    ON security.security_audit_logs (event_type);

CREATE INDEX IF NOT EXISTS ix_security_audit_logs_entity
    ON security.security_audit_logs (entity_type, entity_id);

CREATE INDEX IF NOT EXISTS ix_security_audit_logs_occurred_at
    ON security.security_audit_logs (occurred_at);

-- =========================================================
-- VIEWS
-- =========================================================

CREATE OR REPLACE VIEW security.v_active_seals AS
SELECT
    s.seal_id,
    s.tenant_id,
    s.seal_code,
    s.seal_name,
    s.seal_type,
    s.status,
    s.notary_id,
    n.public_display_name AS notary_name,
    s.state_code,
    s.commission_number,
    s.seal_number,
    s.effective_from,
    s.expires_on,
    s.last_used_at,
    s.usage_count
FROM security.seals s
JOIN identity.notaries n
  ON n.notary_id = s.notary_id
WHERE s.deleted_at IS NULL
  AND n.deleted_at IS NULL;

CREATE OR REPLACE VIEW security.v_certificate_inventory AS
SELECT
    dc.digital_certificate_id,
    dc.tenant_id,
    dc.certificate_code,
    dc.certificate_name,
    dc.status,
    dc.notary_id,
    n.public_display_name AS notary_name,
    dc.provider_name,
    dc.serial_number,
    dc.cryptographic_algorithm,
    dc.key_storage_method,
    dc.valid_from,
    dc.valid_to,
    dc.revoked_at,
    dc.replaced_at
FROM security.digital_certificates dc
JOIN identity.notaries n
  ON n.notary_id = dc.notary_id
WHERE dc.deleted_at IS NULL
  AND n.deleted_at IS NULL;

CREATE OR REPLACE VIEW security.v_security_risk_summary AS
SELECT
    s.tenant_id,
    COUNT(*) FILTER (WHERE s.status = 'active') AS active_seals,
    COUNT(*) FILTER (WHERE s.status IN ('suspended', 'revoked', 'lost', 'stolen')) AS risk_seals,
    COUNT(*) FILTER (WHERE s.expires_on IS NOT NULL AND s.expires_on <= CURRENT_DATE + 30) AS seals_expiring_soon
FROM security.seals s
WHERE s.deleted_at IS NULL
GROUP BY s.tenant_id;

-- =========================================================
-- COMMENTS
-- =========================================================

COMMENT ON SCHEMA security IS 'Security schema for seals, digital certificates, access control, usage logging, incidents, revocation, replacement, and audit.';
COMMENT ON TABLE security.seals IS 'Physical seal and eSeal inventory for each notary.';
COMMENT ON TABLE security.digital_certificates IS 'Digital certificates used for electronic seals and signature workflows.';
COMMENT ON TABLE security.digital_certificate_chain_items IS 'Certificate chain items for validation and audit.';
COMMENT ON TABLE security.seal_access_policies IS 'Who may use a seal or certificate, under what conditions, and with what approvals.';
COMMENT ON TABLE security.trusted_devices IS 'Trusted device registry for access control and step-up authentication.';
COMMENT ON TABLE security.mfa_devices IS 'MFA method registry for user authentication.';
COMMENT ON TABLE security.security_incidents IS 'Security incidents, compromises, unauthorized use, and response lifecycle.';
COMMENT ON TABLE security.security_incident_actions IS 'Action tracking inside a security incident.';
COMMENT ON TABLE security.seal_revocations IS 'Revocation history for seals and digital certificates.';
COMMENT ON TABLE security.seal_replacements IS 'Replacement history for seals and certificates.';
COMMENT ON TABLE security.emergency_locks IS 'One-click suspension / emergency lock records.';
COMMENT ON TABLE security.seal_usage_logs IS 'Traceable usage logs for seal and certificate operations.';
COMMENT ON TABLE security.security_audit_logs IS 'Evidence-grade audit trail for security operations.';


-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================

-- PostgreSQL 15+
-- Billing schema for invoices, invoice items, payments, payment methods,
-- credits, refunds, adjustments, accounts receivable, revenue sharing,
-- commissions payable, aging snapshots, and audit logs.

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

CREATE SCHEMA IF NOT EXISTS billing;

-- =========================================================
-- ENUM TYPES
-- =========================================================

DO $$
BEGIN
    IF to_regtype('billing.invoice_status') IS NULL THEN
        CREATE TYPE billing.invoice_status AS ENUM (
            'draft',
            'issued',
            'partially_paid',
            'paid',
            'overdue',
            'voided',
            'cancelled',
            'written_off'
        );
    END IF;

    IF to_regtype('billing.invoice_item_type') IS NULL THEN
        CREATE TYPE billing.invoice_item_type AS ENUM (
            'service_fee',
            'travel_fee',
            'ron_fee',
            'printing_fee',
            'late_fee',
            'discount',
            'adjustment',
            'tax',
            'other'
        );
    END IF;

    IF to_regtype('billing.payment_status') IS NULL THEN
        CREATE TYPE billing.payment_status AS ENUM (
            'pending',
            'authorized',
            'captured',
            'settled',
            'failed',
            'reversed',
            'refunded',
            'voided'
        );
    END IF;

    IF to_regtype('billing.payment_method_type') IS NULL THEN
        CREATE TYPE billing.payment_method_type AS ENUM (
            'credit_card',
            'ach',
            'cash',
            'check',
            'bank_transfer',
            'wallet',
            'other'
        );
    END IF;

    IF to_regtype('billing.refund_status') IS NULL THEN
        CREATE TYPE billing.refund_status AS ENUM (
            'pending',
            'approved',
            'processed',
            'failed',
            'cancelled'
        );
    END IF;

    IF to_regtype('billing.credit_status') IS NULL THEN
        CREATE TYPE billing.credit_status AS ENUM (
            'available',
            'partially_used',
            'used',
            'expired',
            'voided'
        );
    END IF;

    IF to_regtype('billing.adjustment_status') IS NULL THEN
        CREATE TYPE billing.adjustment_status AS ENUM (
            'draft',
            'posted',
            'reversed',
            'voided'
        );
    END IF;

    IF to_regtype('billing.aging_bucket') IS NULL THEN
        CREATE TYPE billing.aging_bucket AS ENUM (
            'current',
            'days_1_30',
            'days_31_60',
            'days_61_90',
            'days_90_plus'
        );
    END IF;

    IF to_regtype('billing.revenue_share_type') IS NULL THEN
        CREATE TYPE billing.revenue_share_type AS ENUM (
            'percentage',
            'fixed',
            'hybrid',
            'bonus',
            'deduction'
        );
    END IF;

    IF to_regtype('billing.payable_status') IS NULL THEN
        CREATE TYPE billing.payable_status AS ENUM (
            'pending',
            'accrued',
            'approved',
            'paid',
            'partially_paid',
            'cancelled',
            'reversed'
        );
    END IF;

    IF to_regtype('billing.audit_event_type') IS NULL THEN
        CREATE TYPE billing.audit_event_type AS ENUM (
            'create',
            'update',
            'issue_invoice',
            'send_invoice',
            'record_payment',
            'apply_credit',
            'apply_refund',
            'post_adjustment',
            'void_invoice',
            'write_off',
            'accrue_payable',
            'pay_payable',
            'status_change',
            'export',
            'import'
        );
    END IF;
END $$;

-- =========================================================
-- UPDATED_AT TRIGGER
-- =========================================================

CREATE OR REPLACE FUNCTION billing.set_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at = now();
    RETURN NEW;
END;
$$;

-- =========================================================
-- INVOICES
-- =========================================================

CREATE TABLE IF NOT EXISTS billing.invoices (
    invoice_id              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    invoice_code            varchar(50) NOT NULL,
    invoice_number          varchar(100) NOT NULL,
    invoice_status          billing.invoice_status NOT NULL DEFAULT 'draft',

    customer_id             uuid NOT NULL,
    customer_contact_id     uuid,
    contract_id             uuid,
    sla_id                  uuid,

    branch_id               uuid,
    region_id               uuid,

    invoice_date            date NOT NULL DEFAULT CURRENT_DATE,
    due_date                date NOT NULL,
    issued_at               timestamptz,
    sent_at                 timestamptz,
    paid_at                 timestamptz,
    voided_at               timestamptz,
    written_off_at          timestamptz,

    currency_code           char(3) NOT NULL DEFAULT 'USD',
    exchange_rate           numeric(18,6) NOT NULL DEFAULT 1,

    subtotal_amount         numeric(18,2) NOT NULL DEFAULT 0,
    discount_amount         numeric(18,2) NOT NULL DEFAULT 0,
    tax_amount              numeric(18,2) NOT NULL DEFAULT 0,
    total_amount            numeric(18,2) NOT NULL DEFAULT 0,
    paid_amount             numeric(18,2) NOT NULL DEFAULT 0,
    balance_due             numeric(18,2) NOT NULL DEFAULT 0,
    aging_days              integer NOT NULL DEFAULT 0,
    aging_bucket            billing.aging_bucket NOT NULL DEFAULT 'current',

    billing_email           citext,
    billing_phone           varchar(30),
    billing_address_line1    varchar(200),
    billing_address_line2    varchar(200),
    billing_city            varchar(100),
    billing_state_code      char(2),
    billing_postal_code     varchar(20),
    billing_country_code    char(2),

    send_method             varchar(50),
    external_reference      text,
    notes                   text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_invoices_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_invoices_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_invoices_customer_contact
        FOREIGN KEY (customer_contact_id) REFERENCES crm.customer_contacts (contact_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_invoices_contract
        FOREIGN KEY (contract_id) REFERENCES crm.contracts (contract_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_invoices_sla
        FOREIGN KEY (sla_id) REFERENCES crm.sla_agreements (sla_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_invoices_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_invoices_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_invoices_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_invoices_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_invoices_tenant_code
        UNIQUE (tenant_id, invoice_code),

    CONSTRAINT uq_invoices_tenant_number
        UNIQUE (tenant_id, invoice_number),

    CONSTRAINT chk_invoices_amounts_non_negative
        CHECK (
            subtotal_amount >= 0 AND
            discount_amount >= 0 AND
            tax_amount >= 0 AND
            total_amount >= 0 AND
            paid_amount >= 0 AND
            balance_due >= 0
        ),

    CONSTRAINT chk_invoices_exchange_rate
        CHECK (exchange_rate > 0),

    CONSTRAINT chk_invoices_due_after_date
        CHECK (due_date >= invoice_date)
);

CREATE INDEX IF NOT EXISTS ix_invoices_tenant_id
    ON billing.invoices (tenant_id);

CREATE INDEX IF NOT EXISTS ix_invoices_customer_id
    ON billing.invoices (customer_id);

CREATE INDEX IF NOT EXISTS ix_invoices_contract_id
    ON billing.invoices (contract_id);

CREATE INDEX IF NOT EXISTS ix_invoices_status
    ON billing.invoices (invoice_status);

CREATE INDEX IF NOT EXISTS ix_invoices_invoice_date
    ON billing.invoices (invoice_date);

CREATE INDEX IF NOT EXISTS ix_invoices_due_date
    ON billing.invoices (due_date);

CREATE INDEX IF NOT EXISTS ix_invoices_aging_bucket
    ON billing.invoices (aging_bucket);

DROP TRIGGER IF EXISTS trg_invoices_updated_at ON billing.invoices;
CREATE TRIGGER trg_invoices_updated_at
BEFORE UPDATE ON billing.invoices
FOR EACH ROW EXECUTE FUNCTION billing.set_updated_at();

-- =========================================================
-- INVOICE ITEMS
-- =========================================================

CREATE TABLE IF NOT EXISTS billing.invoice_items (
    invoice_item_id         uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    invoice_id              uuid NOT NULL,

    line_no                 integer NOT NULL,
    item_type               billing.invoice_item_type NOT NULL DEFAULT 'other',
    service_type_id         uuid,
    job_id                  uuid,
    notarial_act_id         uuid,
    journal_entry_id        uuid,

    item_code               varchar(50),
    description             varchar(300) NOT NULL,
    quantity                numeric(18,4) NOT NULL DEFAULT 1,
    unit_price              numeric(18,2) NOT NULL DEFAULT 0,
    discount_amount         numeric(18,2) NOT NULL DEFAULT 0,
    tax_amount              numeric(18,2) NOT NULL DEFAULT 0,
    line_total              numeric(18,2) NOT NULL DEFAULT 0,

    unit_of_measure         varchar(50),
    notes                   text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_invoice_items_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_invoice_items_invoice
        FOREIGN KEY (invoice_id) REFERENCES billing.invoices (invoice_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_invoice_items_service_type
        FOREIGN KEY (service_type_id) REFERENCES operations.service_types (service_type_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_invoice_items_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_invoice_items_notarial_act
        FOREIGN KEY (notarial_act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_invoice_items_journal_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_invoice_items_invoice_line
        UNIQUE (invoice_id, line_no),

    CONSTRAINT chk_invoice_items_non_negative
        CHECK (
            quantity >= 0 AND
            unit_price >= 0 AND
            discount_amount >= 0 AND
            tax_amount >= 0 AND
            line_total >= 0
        )
);

CREATE INDEX IF NOT EXISTS ix_invoice_items_tenant_id
    ON billing.invoice_items (tenant_id);

CREATE INDEX IF NOT EXISTS ix_invoice_items_invoice_id
    ON billing.invoice_items (invoice_id);

CREATE INDEX IF NOT EXISTS ix_invoice_items_item_type
    ON billing.invoice_items (item_type);

CREATE INDEX IF NOT EXISTS ix_invoice_items_job_id
    ON billing.invoice_items (job_id);

CREATE INDEX IF NOT EXISTS ix_invoice_items_notarial_act_id
    ON billing.invoice_items (notarial_act_id);

DROP TRIGGER IF EXISTS trg_invoice_items_updated_at ON billing.invoice_items;
CREATE TRIGGER trg_invoice_items_updated_at
BEFORE UPDATE ON billing.invoice_items
FOR EACH ROW EXECUTE FUNCTION billing.set_updated_at();

-- =========================================================
-- PAYMENT METHODS
-- =========================================================

CREATE TABLE IF NOT EXISTS billing.payment_methods (
    payment_method_id       uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    customer_id             uuid NOT NULL,
    customer_contact_id     uuid,

    method_code             varchar(50) NOT NULL,
    method_type             billing.payment_method_type NOT NULL,
    status                  billing.payment_status NOT NULL DEFAULT 'pending',

    provider_name           varchar(200),
    provider_customer_ref   text,
    provider_method_ref     text,
    display_name            varchar(200),

    last4                   varchar(4),
    card_brand              varchar(50),
    exp_month               integer,
    exp_year                integer,

    bank_account_mask       varchar(20),
    bank_routing_mask       varchar(20),

    is_default              boolean NOT NULL DEFAULT false,
    is_verified             boolean NOT NULL DEFAULT false,
    verified_at             timestamptz,
    tokenized_at            timestamptz,
    revoked_at              timestamptz,

    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_payment_methods_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_payment_methods_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_payment_methods_customer_contact
        FOREIGN KEY (customer_contact_id) REFERENCES crm.customer_contacts (contact_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_payment_methods_customer_code
        UNIQUE (customer_id, method_code),

    CONSTRAINT chk_payment_methods_card_month
        CHECK (exp_month IS NULL OR (exp_month >= 1 AND exp_month <= 12)),

    CONSTRAINT chk_payment_methods_card_year
        CHECK (exp_year IS NULL OR exp_year >= 2000)
);

CREATE INDEX IF NOT EXISTS ix_payment_methods_tenant_id
    ON billing.payment_methods (tenant_id);

CREATE INDEX IF NOT EXISTS ix_payment_methods_customer_id
    ON billing.payment_methods (customer_id);

CREATE INDEX IF NOT EXISTS ix_payment_methods_method_type
    ON billing.payment_methods (method_type);

CREATE INDEX IF NOT EXISTS ix_payment_methods_status
    ON billing.payment_methods (status);

DROP TRIGGER IF EXISTS trg_payment_methods_updated_at ON billing.payment_methods;
CREATE TRIGGER trg_payment_methods_updated_at
BEFORE UPDATE ON billing.payment_methods
FOR EACH ROW EXECUTE FUNCTION billing.set_updated_at();

CREATE UNIQUE INDEX IF NOT EXISTS ux_payment_methods_one_default
    ON billing.payment_methods (customer_id)
    WHERE is_default = true AND deleted_at IS NULL;

-- =========================================================
-- PAYMENTS
-- =========================================================

CREATE TABLE IF NOT EXISTS billing.payments (
    payment_id              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    payment_code            varchar(50) NOT NULL,
    payment_status          billing.payment_status NOT NULL DEFAULT 'pending',

    customer_id             uuid NOT NULL,
    invoice_id              uuid,
    payment_method_id       uuid,

    payment_date            date NOT NULL DEFAULT CURRENT_DATE,
    payment_time            timestamptz NOT NULL DEFAULT now(),
    currency_code           char(3) NOT NULL DEFAULT 'USD',
    amount                  numeric(18,2) NOT NULL DEFAULT 0,
    fee_amount              numeric(18,2) NOT NULL DEFAULT 0,
    net_amount              numeric(18,2) NOT NULL DEFAULT 0,

    provider_name           varchar(200),
    provider_transaction_ref text,
    authorization_code      text,
    settlement_batch_ref    text,

    captured_at             timestamptz,
    settled_at              timestamptz,
    failed_at               timestamptz,
    reversed_at             timestamptz,

    failure_reason          text,
    notes                   text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_payments_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_payments_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_payments_invoice
        FOREIGN KEY (invoice_id) REFERENCES billing.invoices (invoice_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_payments_payment_method
        FOREIGN KEY (payment_method_id) REFERENCES billing.payment_methods (payment_method_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_payments_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_payments_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_payments_tenant_code
        UNIQUE (tenant_id, payment_code),

    CONSTRAINT chk_payments_amounts_non_negative
        CHECK (amount >= 0 AND fee_amount >= 0 AND net_amount >= 0)
);

CREATE INDEX IF NOT EXISTS ix_payments_tenant_id
    ON billing.payments (tenant_id);

CREATE INDEX IF NOT EXISTS ix_payments_customer_id
    ON billing.payments (customer_id);

CREATE INDEX IF NOT EXISTS ix_payments_invoice_id
    ON billing.payments (invoice_id);

CREATE INDEX IF NOT EXISTS ix_payments_status
    ON billing.payments (payment_status);

CREATE INDEX IF NOT EXISTS ix_payments_payment_date
    ON billing.payments (payment_date);

DROP TRIGGER IF EXISTS trg_payments_updated_at ON billing.payments;
CREATE TRIGGER trg_payments_updated_at
BEFORE UPDATE ON billing.payments
FOR EACH ROW EXECUTE FUNCTION billing.set_updated_at();

-- =========================================================
-- PAYMENT ALLOCATIONS
-- Supports partial payments and one payment to many invoices
-- =========================================================

CREATE TABLE IF NOT EXISTS billing.payment_allocations (
    payment_allocation_id   uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    payment_id              uuid NOT NULL,
    invoice_id              uuid NOT NULL,

    allocation_code         varchar(50) NOT NULL,
    allocated_amount        numeric(18,2) NOT NULL DEFAULT 0,
    applied_at              timestamptz NOT NULL DEFAULT now(),
    notes                   text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_payment_allocations_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_payment_allocations_payment
        FOREIGN KEY (payment_id) REFERENCES billing.payments (payment_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_payment_allocations_invoice
        FOREIGN KEY (invoice_id) REFERENCES billing.invoices (invoice_id)
        ON DELETE CASCADE,

    CONSTRAINT uq_payment_allocations_tenant_code
        UNIQUE (tenant_id, allocation_code),

    CONSTRAINT uq_payment_allocations_payment_invoice
        UNIQUE (payment_id, invoice_id),

    CONSTRAINT chk_payment_allocations_amount
        CHECK (allocated_amount >= 0)
);

CREATE INDEX IF NOT EXISTS ix_payment_allocations_tenant_id
    ON billing.payment_allocations (tenant_id);

CREATE INDEX IF NOT EXISTS ix_payment_allocations_payment_id
    ON billing.payment_allocations (payment_id);

CREATE INDEX IF NOT EXISTS ix_payment_allocations_invoice_id
    ON billing.payment_allocations (invoice_id);

DROP TRIGGER IF EXISTS trg_payment_allocations_updated_at ON billing.payment_allocations;
CREATE TRIGGER trg_payment_allocations_updated_at
BEFORE UPDATE ON billing.payment_allocations
FOR EACH ROW EXECUTE FUNCTION billing.set_updated_at();

-- =========================================================
-- CREDITS
-- =========================================================

CREATE TABLE IF NOT EXISTS billing.credits (
    credit_id               uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    credit_code             varchar(50) NOT NULL,
    customer_id             uuid NOT NULL,
    invoice_id              uuid,
    origin_payment_id       uuid,

    credit_status           billing.credit_status NOT NULL DEFAULT 'available',
    credit_date             date NOT NULL DEFAULT CURRENT_DATE,
    currency_code           char(3) NOT NULL DEFAULT 'USD',

    original_amount         numeric(18,2) NOT NULL DEFAULT 0,
    available_amount        numeric(18,2) NOT NULL DEFAULT 0,
    used_amount             numeric(18,2) NOT NULL DEFAULT 0,
    expired_amount          numeric(18,2) NOT NULL DEFAULT 0,

    expires_on              date,
    reason                  text,
    notes                   text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_credits_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_credits_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_credits_invoice
        FOREIGN KEY (invoice_id) REFERENCES billing.invoices (invoice_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_credits_origin_payment
        FOREIGN KEY (origin_payment_id) REFERENCES billing.payments (payment_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_credits_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_credits_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_credits_tenant_code
        UNIQUE (tenant_id, credit_code),

    CONSTRAINT chk_credits_amounts_non_negative
        CHECK (
            original_amount >= 0 AND
            available_amount >= 0 AND
            used_amount >= 0 AND
            expired_amount >= 0
        )
);

CREATE INDEX IF NOT EXISTS ix_credits_tenant_id
    ON billing.credits (tenant_id);

CREATE INDEX IF NOT EXISTS ix_credits_customer_id
    ON billing.credits (customer_id);

CREATE INDEX IF NOT EXISTS ix_credits_credit_status
    ON billing.credits (credit_status);

CREATE INDEX IF NOT EXISTS ix_credits_expires_on
    ON billing.credits (expires_on);

DROP TRIGGER IF EXISTS trg_credits_updated_at ON billing.credits;
CREATE TRIGGER trg_credits_updated_at
BEFORE UPDATE ON billing.credits
FOR EACH ROW EXECUTE FUNCTION billing.set_updated_at();

-- =========================================================
-- CREDIT APPLICATIONS
-- =========================================================

CREATE TABLE IF NOT EXISTS billing.credit_applications (
    credit_application_id   uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    credit_id               uuid NOT NULL,
    invoice_id              uuid NOT NULL,

    application_code        varchar(50) NOT NULL,
    applied_amount          numeric(18,2) NOT NULL DEFAULT 0,
    applied_at              timestamptz NOT NULL DEFAULT now(),
    notes                   text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_credit_applications_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_credit_applications_credit
        FOREIGN KEY (credit_id) REFERENCES billing.credits (credit_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_credit_applications_invoice
        FOREIGN KEY (invoice_id) REFERENCES billing.invoices (invoice_id)
        ON DELETE CASCADE,

    CONSTRAINT uq_credit_applications_tenant_code
        UNIQUE (tenant_id, application_code),

    CONSTRAINT uq_credit_applications_credit_invoice
        UNIQUE (credit_id, invoice_id),

    CONSTRAINT chk_credit_applications_amount
        CHECK (applied_amount >= 0)
);

CREATE INDEX IF NOT EXISTS ix_credit_applications_tenant_id
    ON billing.credit_applications (tenant_id);

CREATE INDEX IF NOT EXISTS ix_credit_applications_credit_id
    ON billing.credit_applications (credit_id);

CREATE INDEX IF NOT EXISTS ix_credit_applications_invoice_id
    ON billing.credit_applications (invoice_id);

DROP TRIGGER IF EXISTS trg_credit_applications_updated_at ON billing.credit_applications;
CREATE TRIGGER trg_credit_applications_updated_at
BEFORE UPDATE ON billing.credit_applications
FOR EACH ROW EXECUTE FUNCTION billing.set_updated_at();

-- =========================================================
-- REFUNDS
-- =========================================================

CREATE TABLE IF NOT EXISTS billing.refunds (
    refund_id               uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    refund_code             varchar(50) NOT NULL,
    refund_status           billing.refund_status NOT NULL DEFAULT 'pending',

    payment_id              uuid NOT NULL,
    invoice_id              uuid,
    customer_id             uuid NOT NULL,

    refund_date             date NOT NULL DEFAULT CURRENT_DATE,
    currency_code           char(3) NOT NULL DEFAULT 'USD',
    amount                  numeric(18,2) NOT NULL DEFAULT 0,
    reason                  text NOT NULL,
    notes                   text,

    provider_refund_ref     text,
    processed_at            timestamptz,
    failed_at               timestamptz,
    failure_reason          text,

    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_refunds_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_refunds_payment
        FOREIGN KEY (payment_id) REFERENCES billing.payments (payment_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_refunds_invoice
        FOREIGN KEY (invoice_id) REFERENCES billing.invoices (invoice_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_refunds_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_refunds_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_refunds_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_refunds_tenant_code
        UNIQUE (tenant_id, refund_code),

    CONSTRAINT chk_refunds_amount
        CHECK (amount >= 0)
);

CREATE INDEX IF NOT EXISTS ix_refunds_tenant_id
    ON billing.refunds (tenant_id);

CREATE INDEX IF NOT EXISTS ix_refunds_payment_id
    ON billing.refunds (payment_id);

CREATE INDEX IF NOT EXISTS ix_refunds_customer_id
    ON billing.refunds (customer_id);

CREATE INDEX IF NOT EXISTS ix_refunds_status
    ON billing.refunds (refund_status);

DROP TRIGGER IF EXISTS trg_refunds_updated_at ON billing.refunds;
CREATE TRIGGER trg_refunds_updated_at
BEFORE UPDATE ON billing.refunds
FOR EACH ROW EXECUTE FUNCTION billing.set_updated_at();

-- =========================================================
-- BILLING ADJUSTMENTS
-- =========================================================

CREATE TABLE IF NOT EXISTS billing.billing_adjustments (
    billing_adjustment_id   uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    adjustment_code         varchar(50) NOT NULL,
    adjustment_status       billing.adjustment_status NOT NULL DEFAULT 'draft',
    adjustment_type         billing.invoice_item_type NOT NULL DEFAULT 'adjustment',

    invoice_id              uuid,
    payment_id              uuid,
    customer_id             uuid NOT NULL,

    adjustment_date         date NOT NULL DEFAULT CURRENT_DATE,
    currency_code           char(3) NOT NULL DEFAULT 'USD',
    amount                  numeric(18,2) NOT NULL DEFAULT 0,

    reason                  text NOT NULL,
    notes                   text,
    source_reference        text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_billing_adjustments_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_billing_adjustments_invoice
        FOREIGN KEY (invoice_id) REFERENCES billing.invoices (invoice_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_billing_adjustments_payment
        FOREIGN KEY (payment_id) REFERENCES billing.payments (payment_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_billing_adjustments_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_billing_adjustments_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_billing_adjustments_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_billing_adjustments_tenant_code
        UNIQUE (tenant_id, adjustment_code),

    CONSTRAINT chk_billing_adjustments_amount
        CHECK (amount >= 0)
);

CREATE INDEX IF NOT EXISTS ix_billing_adjustments_tenant_id
    ON billing.billing_adjustments (tenant_id);

CREATE INDEX IF NOT EXISTS ix_billing_adjustments_invoice_id
    ON billing.billing_adjustments (invoice_id);

CREATE INDEX IF NOT EXISTS ix_billing_adjustments_customer_id
    ON billing.billing_adjustments (customer_id);

CREATE INDEX IF NOT EXISTS ix_billing_adjustments_status
    ON billing.billing_adjustments (adjustment_status);

DROP TRIGGER IF EXISTS trg_billing_adjustments_updated_at ON billing.billing_adjustments;
CREATE TRIGGER trg_billing_adjustments_updated_at
BEFORE UPDATE ON billing.billing_adjustments
FOR EACH ROW EXECUTE FUNCTION billing.set_updated_at();

-- =========================================================
-- ACCOUNTS RECEIVABLE SNAPSHOTS
-- =========================================================

CREATE TABLE IF NOT EXISTS billing.accounts_receivable_snapshots (
    ar_snapshot_id          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    snapshot_date           date NOT NULL,
    customer_id             uuid,
    branch_id               uuid,
    region_id               uuid,

    current_balance         numeric(18,2) NOT NULL DEFAULT 0,
    days_1_30_balance       numeric(18,2) NOT NULL DEFAULT 0,
    days_31_60_balance      numeric(18,2) NOT NULL DEFAULT 0,
    days_61_90_balance      numeric(18,2) NOT NULL DEFAULT 0,
    days_90_plus_balance    numeric(18,2) NOT NULL DEFAULT 0,

    invoice_count           integer NOT NULL DEFAULT 0,
    overdue_invoice_count   integer NOT NULL DEFAULT 0,
    metrics                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_ar_snapshots_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_ar_snapshots_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_ar_snapshots_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_ar_snapshots_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_ar_snapshots_unique_scope
        UNIQUE (tenant_id, snapshot_date, customer_id, branch_id, region_id),

    CONSTRAINT chk_ar_snapshots_balances
        CHECK (
            current_balance >= 0 AND
            days_1_30_balance >= 0 AND
            days_31_60_balance >= 0 AND
            days_61_90_balance >= 0 AND
            days_90_plus_balance >= 0 AND
            invoice_count >= 0 AND
            overdue_invoice_count >= 0
        )
);

CREATE INDEX IF NOT EXISTS ix_ar_snapshots_tenant_id
    ON billing.accounts_receivable_snapshots (tenant_id);

CREATE INDEX IF NOT EXISTS ix_ar_snapshots_snapshot_date
    ON billing.accounts_receivable_snapshots (snapshot_date);

CREATE INDEX IF NOT EXISTS ix_ar_snapshots_customer_id
    ON billing.accounts_receivable_snapshots (customer_id);

DROP TRIGGER IF EXISTS trg_ar_snapshots_updated_at ON billing.accounts_receivable_snapshots;
CREATE TRIGGER trg_ar_snapshots_updated_at
BEFORE UPDATE ON billing.accounts_receivable_snapshots
FOR EACH ROW EXECUTE FUNCTION billing.set_updated_at();

-- =========================================================
-- REVENUE SHARES
-- =========================================================

CREATE TABLE IF NOT EXISTS billing.revenue_shares (
    revenue_share_id        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    revenue_share_code      varchar(50) NOT NULL,
    revenue_share_type      billing.revenue_share_type NOT NULL DEFAULT 'percentage',
    status                  billing.payable_status NOT NULL DEFAULT 'pending',

    customer_id             uuid,
    contract_id             uuid,
    invoice_id              uuid,
    job_id                  uuid,
    notarial_act_id         uuid,
    journal_entry_id        uuid,
    notary_id               uuid,

    share_basis_amount      numeric(18,2) NOT NULL DEFAULT 0,
    share_rate_percent      numeric(5,2),
    fixed_share_amount      numeric(18,2),
    gross_share_amount      numeric(18,2) NOT NULL DEFAULT 0,
    deductions_amount       numeric(18,2) NOT NULL DEFAULT 0,
    net_share_amount        numeric(18,2) NOT NULL DEFAULT 0,

    accrued_at              timestamptz,
    approved_at             timestamptz,
    paid_at                 timestamptz,
    reversed_at             timestamptz,

    notes                   text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_revenue_shares_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_revenue_shares_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_revenue_shares_contract
        FOREIGN KEY (contract_id) REFERENCES crm.contracts (contract_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_revenue_shares_invoice
        FOREIGN KEY (invoice_id) REFERENCES billing.invoices (invoice_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_revenue_shares_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_revenue_shares_notarial_act
        FOREIGN KEY (notarial_act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_revenue_shares_journal_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_revenue_shares_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_revenue_shares_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_revenue_shares_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_revenue_shares_tenant_code
        UNIQUE (tenant_id, revenue_share_code),

    CONSTRAINT chk_revenue_shares_amounts
        CHECK (
            share_basis_amount >= 0 AND
            gross_share_amount >= 0 AND
            deductions_amount >= 0 AND
            net_share_amount >= 0 AND
            (share_rate_percent IS NULL OR (share_rate_percent >= 0 AND share_rate_percent <= 100)) AND
            (fixed_share_amount IS NULL OR fixed_share_amount >= 0)
        )
);

CREATE INDEX IF NOT EXISTS ix_revenue_shares_tenant_id
    ON billing.revenue_shares (tenant_id);

CREATE INDEX IF NOT EXISTS ix_revenue_shares_status
    ON billing.revenue_shares (status);

CREATE INDEX IF NOT EXISTS ix_revenue_shares_notary_id
    ON billing.revenue_shares (notary_id);

CREATE INDEX IF NOT EXISTS ix_revenue_shares_invoice_id
    ON billing.revenue_shares (invoice_id);

DROP TRIGGER IF EXISTS trg_revenue_shares_updated_at ON billing.revenue_shares;
CREATE TRIGGER trg_revenue_shares_updated_at
BEFORE UPDATE ON billing.revenue_shares
FOR EACH ROW EXECUTE FUNCTION billing.set_updated_at();

-- =========================================================
-- NOTARY COMMISSIONS PAYABLE
-- =========================================================

CREATE TABLE IF NOT EXISTS billing.notary_commissions_payable (
    payable_id              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    payable_code            varchar(50) NOT NULL,
    payable_status          billing.payable_status NOT NULL DEFAULT 'pending',

    notary_id               uuid NOT NULL,
    revenue_share_id        uuid,
    invoice_id              uuid,
    payment_id              uuid,

    accrual_date            date NOT NULL DEFAULT CURRENT_DATE,
    due_date                date,
    currency_code           char(3) NOT NULL DEFAULT 'USD',

    gross_amount            numeric(18,2) NOT NULL DEFAULT 0,
    deductions_amount       numeric(18,2) NOT NULL DEFAULT 0,
    net_amount              numeric(18,2) NOT NULL DEFAULT 0,
    paid_amount             numeric(18,2) NOT NULL DEFAULT 0,
    balance_due             numeric(18,2) NOT NULL DEFAULT 0,

    payment_method_text     varchar(100),
    payout_reference        text,
    notes                   text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    approved_by_user_id     uuid,
    paid_by_user_id         uuid,
    approved_at             timestamptz,
    paid_at                 timestamptz,
    reversed_at             timestamptz,

    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_payables_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_payables_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_payables_revenue_share
        FOREIGN KEY (revenue_share_id) REFERENCES billing.revenue_shares (revenue_share_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_payables_invoice
        FOREIGN KEY (invoice_id) REFERENCES billing.invoices (invoice_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_payables_payment
        FOREIGN KEY (payment_id) REFERENCES billing.payments (payment_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_payables_approved_by
        FOREIGN KEY (approved_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_payables_paid_by
        FOREIGN KEY (paid_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_payables_tenant_code
        UNIQUE (tenant_id, payable_code),

    CONSTRAINT chk_payables_amounts
        CHECK (
            gross_amount >= 0 AND
            deductions_amount >= 0 AND
            net_amount >= 0 AND
            paid_amount >= 0 AND
            balance_due >= 0
        )
);

CREATE INDEX IF NOT EXISTS ix_notary_commissions_payable_tenant_id
    ON billing.notary_commissions_payable (tenant_id);

CREATE INDEX IF NOT EXISTS ix_notary_commissions_payable_notary_id
    ON billing.notary_commissions_payable (notary_id);

CREATE INDEX IF NOT EXISTS ix_notary_commissions_payable_status
    ON billing.notary_commissions_payable (payable_status);

CREATE INDEX IF NOT EXISTS ix_notary_commissions_payable_due_date
    ON billing.notary_commissions_payable (due_date);

DROP TRIGGER IF EXISTS trg_notary_commissions_payable_updated_at ON billing.notary_commissions_payable;
CREATE TRIGGER trg_notary_commissions_payable_updated_at
BEFORE UPDATE ON billing.notary_commissions_payable
FOR EACH ROW EXECUTE FUNCTION billing.set_updated_at();

-- =========================================================
-- BILLING AUDIT LOGS
-- =========================================================

CREATE TABLE IF NOT EXISTS billing.billing_audit_logs (
    billing_audit_log_id    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    audit_code              varchar(50) NOT NULL,
    event_type              billing.audit_event_type NOT NULL,
    entity_type             varchar(100) NOT NULL,
    entity_id               uuid,
    entity_code             varchar(100),

    actor_user_id           uuid,
    source_ip               inet,
    user_agent              text,

    before_data             jsonb,
    after_data              jsonb,
    occurred_at             timestamptz NOT NULL DEFAULT now(),
    source_reference        text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_billing_audit_logs_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_billing_audit_logs_actor_user
        FOREIGN KEY (actor_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_billing_audit_logs_tenant_code
        UNIQUE (tenant_id, audit_code)
);

CREATE INDEX IF NOT EXISTS ix_billing_audit_logs_tenant_id
    ON billing.billing_audit_logs (tenant_id);

CREATE INDEX IF NOT EXISTS ix_billing_audit_logs_event_type
    ON billing.billing_audit_logs (event_type);

CREATE INDEX IF NOT EXISTS ix_billing_audit_logs_entity
    ON billing.billing_audit_logs (entity_type, entity_id);

CREATE INDEX IF NOT EXISTS ix_billing_audit_logs_occurred_at
    ON billing.billing_audit_logs (occurred_at);

-- =========================================================
-- OPTIONAL TRIGGER: INVOICE AGGREGATION RECALC
-- =========================================================

CREATE OR REPLACE FUNCTION billing.recalculate_invoice_totals()
RETURNS trigger
LANGUAGE plpgsql
AS $$
DECLARE
    v_invoice_id uuid;
BEGIN
    v_invoice_id := COALESCE(NEW.invoice_id, OLD.invoice_id);

    UPDATE billing.invoices i
    SET
        subtotal_amount = COALESCE(x.subtotal_amount, 0),
        discount_amount = COALESCE(x.discount_amount, 0),
        tax_amount = COALESCE(x.tax_amount, 0),
        total_amount = COALESCE(x.total_amount, 0),
        paid_amount = COALESCE(p.paid_amount, 0),
        balance_due = GREATEST(COALESCE(x.total_amount, 0) - COALESCE(p.paid_amount, 0), 0),
        updated_at = now()
    FROM (
        SELECT
            ii.invoice_id,
            SUM((ii.quantity * ii.unit_price))::numeric(18,2) AS subtotal_amount,
            SUM(ii.discount_amount)::numeric(18,2) AS discount_amount,
            SUM(ii.tax_amount)::numeric(18,2) AS tax_amount,
            SUM(ii.line_total)::numeric(18,2) AS total_amount
        FROM billing.invoice_items ii
        WHERE ii.invoice_id = v_invoice_id
          AND ii.deleted_at IS NULL
        GROUP BY ii.invoice_id
    ) x
    LEFT JOIN (
        SELECT
            pa.invoice_id,
            SUM(pa.allocated_amount)::numeric(18,2) AS paid_amount
        FROM billing.payment_allocations pa
        WHERE pa.invoice_id = v_invoice_id
          AND pa.deleted_at IS NULL
        GROUP BY pa.invoice_id
    ) p ON p.invoice_id = x.invoice_id
    WHERE i.invoice_id = v_invoice_id;

    RETURN NULL;
END;
$$;

DROP TRIGGER IF EXISTS trg_invoice_items_recalc_invoice ON billing.invoice_items;
CREATE TRIGGER trg_invoice_items_recalc_invoice
AFTER INSERT OR UPDATE OR DELETE ON billing.invoice_items
FOR EACH ROW EXECUTE FUNCTION billing.recalculate_invoice_totals();

DROP TRIGGER IF EXISTS trg_payment_allocations_recalc_invoice ON billing.payment_allocations;
CREATE TRIGGER trg_payment_allocations_recalc_invoice
AFTER INSERT OR UPDATE OR DELETE ON billing.payment_allocations
FOR EACH ROW EXECUTE FUNCTION billing.recalculate_invoice_totals();

-- =========================================================
-- VIEWS
-- =========================================================

CREATE OR REPLACE VIEW billing.v_invoice_overview AS
SELECT
    i.invoice_id,
    i.tenant_id,
    i.invoice_code,
    i.invoice_number,
    i.invoice_status,
    i.customer_id,
    c.display_name AS customer_name,
    i.contract_id,
    i.sla_id,
    i.invoice_date,
    i.due_date,
    i.currency_code,
    i.subtotal_amount,
    i.discount_amount,
    i.tax_amount,
    i.total_amount,
    i.paid_amount,
    i.balance_due,
    i.aging_days,
    i.aging_bucket,
    i.issued_at,
    i.sent_at,
    i.paid_at,
    i.voided_at
FROM billing.invoices i
JOIN crm.customers c
  ON c.customer_id = i.customer_id
WHERE i.deleted_at IS NULL
  AND c.deleted_at IS NULL;

CREATE OR REPLACE VIEW billing.v_aging_summary AS
SELECT
    i.tenant_id,
    i.customer_id,
    i.aging_bucket,
    COUNT(*) AS invoice_count,
    SUM(i.balance_due) AS total_balance_due
FROM billing.invoices i
WHERE i.deleted_at IS NULL
GROUP BY i.tenant_id, i.customer_id, i.aging_bucket;

CREATE OR REPLACE VIEW billing.v_revenue_share_summary AS
SELECT
    rs.tenant_id,
    rs.notary_id,
    n.public_display_name AS notary_name,
    rs.status,
    COUNT(*) AS share_count,
    SUM(rs.net_share_amount) AS total_net_share
FROM billing.revenue_shares rs
LEFT JOIN identity.notaries n
  ON n.notary_id = rs.notary_id
WHERE rs.deleted_at IS NULL
  AND (n.deleted_at IS NULL OR n.notary_id IS NULL)
GROUP BY rs.tenant_id, rs.notary_id, n.public_display_name, rs.status;

-- =========================================================
-- COMMENTS
-- =========================================================

COMMENT ON SCHEMA billing IS 'Billing schema for invoices, payments, credits, refunds, AR, revenue sharing, commissions payable, and audit logs.';
COMMENT ON TABLE billing.invoices IS 'Customer invoices and AR head record.';
COMMENT ON TABLE billing.invoice_items IS 'Invoice line items linked to jobs, acts, and journal entries.';
COMMENT ON TABLE billing.payment_methods IS 'Stored customer payment methods.';
COMMENT ON TABLE billing.payments IS 'Payment transactions and gateway references.';
COMMENT ON TABLE billing.payment_allocations IS 'Allocations of payments to invoices.';
COMMENT ON TABLE billing.credits IS 'Customer credit balances and credit memos.';
COMMENT ON TABLE billing.credit_applications IS 'Application of credits to invoices.';
COMMENT ON TABLE billing.refunds IS 'Refund transactions and statuses.';
COMMENT ON TABLE billing.billing_adjustments IS 'Manual billing adjustments and corrections.';
COMMENT ON TABLE billing.accounts_receivable_snapshots IS 'AR aging snapshots for dashboards and reporting.';
COMMENT ON TABLE billing.revenue_shares IS 'Revenue share calculations for jobs, acts, invoices, and notaries.';
COMMENT ON TABLE billing.notary_commissions_payable IS 'Commission payable records for notaries.';
COMMENT ON TABLE billing.billing_audit_logs IS 'Evidence-grade billing audit trail.';


-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================

-- PostgreSQL 15+
-- Communication schema for interaction timelines, threads, messages,
-- participants, attachments, templates, reminders, delivery logs, notes, and call logs.

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

CREATE SCHEMA IF NOT EXISTS communication;

-- =========================================================
-- ENUM TYPES
-- =========================================================

DO $$
BEGIN
    IF to_regtype('communication.channel_type') IS NULL THEN
        CREATE TYPE communication.channel_type AS ENUM (
            'email',
            'sms',
            'phone',
            'meeting',
            'in_app',
            'whatsapp',
            'other'
        );
    END IF;

    IF to_regtype('communication.thread_status') IS NULL THEN
        CREATE TYPE communication.thread_status AS ENUM (
            'open',
            'pending',
            'closed',
            'archived'
        );
    END IF;

    IF to_regtype('communication.message_direction') IS NULL THEN
        CREATE TYPE communication.message_direction AS ENUM (
            'inbound',
            'outbound',
            'internal'
        );
    END IF;

    IF to_regtype('communication.message_status') IS NULL THEN
        CREATE TYPE communication.message_status AS ENUM (
            'draft',
            'queued',
            'sent',
            'delivered',
            'read',
            'failed',
            'cancelled'
        );
    END IF;

    IF to_regtype('communication.participant_role') IS NULL THEN
        CREATE TYPE communication.participant_role AS ENUM (
            'customer',
            'customer_contact',
            'user',
            'notary',
            'external',
            'observer'
        );
    END IF;

    IF to_regtype('communication.delivery_status') IS NULL THEN
        CREATE TYPE communication.delivery_status AS ENUM (
            'pending',
            'queued',
            'sent',
            'delivered',
            'failed',
            'bounced',
            'suppressed',
            'cancelled'
        );
    END IF;

    IF to_regtype('communication.reminder_status') IS NULL THEN
        CREATE TYPE communication.reminder_status AS ENUM (
            'pending',
            'queued',
            'sent',
            'delivered',
            'failed',
            'cancelled',
            'snoozed'
        );
    END IF;

    IF to_regtype('communication.note_visibility') IS NULL THEN
        CREATE TYPE communication.note_visibility AS ENUM (
            'private',
            'team',
            'company'
        );
    END IF;

    IF to_regtype('communication.call_status') IS NULL THEN
        CREATE TYPE communication.call_status AS ENUM (
            'scheduled',
            'in_progress',
            'completed',
            'no_answer',
            'voicemail',
            'cancelled',
            'failed'
        );
    END IF;

    IF to_regtype('communication.call_outcome') IS NULL THEN
        CREATE TYPE communication.call_outcome AS ENUM (
            'connected',
            'no_answer',
            'voicemail_left',
            'callback_requested',
            'follow_up_needed',
            'resolved',
            'escalated',
            'other'
        );
    END IF;

    IF to_regtype('communication.template_type') IS NULL THEN
        CREATE TYPE communication.template_type AS ENUM (
            'email',
            'sms',
            'in_app',
            'call_script',
            'meeting_agenda',
            'other'
        );
    END IF;

    IF to_regtype('communication.audit_event_type') IS NULL THEN
        CREATE TYPE communication.audit_event_type AS ENUM (
            'create',
            'update',
            'send',
            'deliver',
            'read',
            'fail',
            'cancel',
            'open_thread',
            'close_thread',
            'archive_thread',
            'add_participant',
            'remove_participant',
            'attach_file',
            'remove_file',
            'create_note',
            'update_note',
            'schedule_reminder',
            'complete_reminder',
            'schedule_call',
            'complete_call'
        );
    END IF;
END $$;

-- =========================================================
-- UPDATED_AT TRIGGER
-- =========================================================

CREATE OR REPLACE FUNCTION communication.set_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at = now();
    RETURN NEW;
END;
$$;

-- =========================================================
-- THREADS
-- One conversation/timeline per customer/account/job/act/etc.
-- =========================================================

CREATE TABLE IF NOT EXISTS communication.communication_threads (
    thread_id               uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    thread_code             varchar(50) NOT NULL,
    channel_type            communication.channel_type NOT NULL DEFAULT 'email',
    thread_status           communication.thread_status NOT NULL DEFAULT 'open',

    subject                 varchar(300),
    summary                 text,
    is_internal             boolean NOT NULL DEFAULT false,
    is_important            boolean NOT NULL DEFAULT false,
    is_pinned               boolean NOT NULL DEFAULT false,

    customer_id             uuid,
    customer_contact_id     uuid,
    job_id                  uuid,
    notarial_act_id         uuid,
    journal_entry_id        uuid,
    invoice_id              uuid,
    payment_id              uuid,
    incident_id             uuid,
    branch_id               uuid,
    region_id               uuid,

    assigned_user_id        uuid,
    assigned_team_id        uuid,
    last_message_at         timestamptz,
    last_activity_at        timestamptz,
    closed_at               timestamptz,
    archived_at             timestamptz,

    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_threads_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_threads_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_threads_customer_contact
        FOREIGN KEY (customer_contact_id) REFERENCES crm.customer_contacts (contact_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_threads_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_threads_notarial_act
        FOREIGN KEY (notarial_act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_threads_journal_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_threads_invoice
        FOREIGN KEY (invoice_id) REFERENCES billing.invoices (invoice_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_threads_payment
        FOREIGN KEY (payment_id) REFERENCES billing.payments (payment_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_threads_incident
        FOREIGN KEY (incident_id) REFERENCES security.security_incidents (incident_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_threads_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_threads_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_threads_assigned_user
        FOREIGN KEY (assigned_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_threads_assigned_team
        FOREIGN KEY (assigned_team_id) REFERENCES core.teams (team_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_threads_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_threads_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_threads_tenant_code
        UNIQUE (tenant_id, thread_code)
);

CREATE INDEX IF NOT EXISTS ix_threads_tenant_id
    ON communication.communication_threads (tenant_id);

CREATE INDEX IF NOT EXISTS ix_threads_channel_type
    ON communication.communication_threads (channel_type);

CREATE INDEX IF NOT EXISTS ix_threads_thread_status
    ON communication.communication_threads (thread_status);

CREATE INDEX IF NOT EXISTS ix_threads_customer_id
    ON communication.communication_threads (customer_id);

CREATE INDEX IF NOT EXISTS ix_threads_job_id
    ON communication.communication_threads (job_id);

CREATE INDEX IF NOT EXISTS ix_threads_notarial_act_id
    ON communication.communication_threads (notarial_act_id);

CREATE INDEX IF NOT EXISTS ix_threads_last_activity_at
    ON communication.communication_threads (last_activity_at);

DROP TRIGGER IF EXISTS trg_threads_updated_at ON communication.communication_threads;
CREATE TRIGGER trg_threads_updated_at
BEFORE UPDATE ON communication.communication_threads
FOR EACH ROW EXECUTE FUNCTION communication.set_updated_at();

-- =========================================================
-- PARTICIPANTS
-- =========================================================

CREATE TABLE IF NOT EXISTS communication.communication_participants (
    participant_id          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    thread_id               uuid NOT NULL,

    participant_code        varchar(50) NOT NULL,
    participant_role        communication.participant_role NOT NULL DEFAULT 'external',

    customer_id             uuid,
    customer_contact_id     uuid,
    user_id                 uuid,
    notary_id               uuid,

    display_name            varchar(200),
    email                   citext,
    phone                   varchar(30),

    is_primary              boolean NOT NULL DEFAULT false,
    is_active               boolean NOT NULL DEFAULT true,
    joined_at               timestamptz NOT NULL DEFAULT now(),
    left_at                 timestamptz,

    notification_email      boolean NOT NULL DEFAULT true,
    notification_sms        boolean NOT NULL DEFAULT false,
    notification_in_app     boolean NOT NULL DEFAULT true,

    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_participants_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_participants_thread
        FOREIGN KEY (thread_id) REFERENCES communication.communication_threads (thread_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_participants_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_participants_customer_contact
        FOREIGN KEY (customer_contact_id) REFERENCES crm.customer_contacts (contact_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_participants_user
        FOREIGN KEY (user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_participants_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_participants_thread_code
        UNIQUE (thread_id, participant_code)
);

CREATE INDEX IF NOT EXISTS ix_participants_tenant_id
    ON communication.communication_participants (tenant_id);

CREATE INDEX IF NOT EXISTS ix_participants_thread_id
    ON communication.communication_participants (thread_id);

CREATE INDEX IF NOT EXISTS ix_participants_role
    ON communication.communication_participants (participant_role);

CREATE INDEX IF NOT EXISTS ix_participants_customer_id
    ON communication.communication_participants (customer_id);

CREATE INDEX IF NOT EXISTS ix_participants_user_id
    ON communication.communication_participants (user_id);

CREATE INDEX IF NOT EXISTS ix_participants_notary_id
    ON communication.communication_participants (notary_id);

CREATE UNIQUE INDEX IF NOT EXISTS ux_participants_one_primary
    ON communication.communication_participants (thread_id)
    WHERE is_primary = true AND deleted_at IS NULL;

DROP TRIGGER IF EXISTS trg_participants_updated_at ON communication.communication_participants;
CREATE TRIGGER trg_participants_updated_at
BEFORE UPDATE ON communication.communication_participants
FOR EACH ROW EXECUTE FUNCTION communication.set_updated_at();

-- =========================================================
-- MESSAGES
-- =========================================================

CREATE TABLE IF NOT EXISTS communication.communication_messages (
    message_id              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    thread_id               uuid NOT NULL,

    message_code            varchar(50) NOT NULL,
    direction               communication.message_direction NOT NULL DEFAULT 'internal',
    message_status          communication.message_status NOT NULL DEFAULT 'draft',

    sender_participant_id   uuid,
    recipient_participant_id uuid,
    sender_user_id          uuid,
    sender_notary_id        uuid,

    subject                 varchar(300),
    body_text               text,
    body_html               text,

    sent_at                 timestamptz,
    delivered_at            timestamptz,
    read_at                 timestamptz,
    failed_at               timestamptz,
    failure_reason          text,

    external_message_id     text,
    provider_name           varchar(200),
    provider_status         varchar(100),
    source_channel          communication.channel_type NOT NULL DEFAULT 'in_app',

    is_important            boolean NOT NULL DEFAULT false,
    is_internal             boolean NOT NULL DEFAULT false,
    is_system_generated     boolean NOT NULL DEFAULT false,

    linked_entity_type      varchar(100),
    linked_entity_id        uuid,
    linked_entity_code      varchar(100),

    reply_to_message_id     uuid,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_messages_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_messages_thread
        FOREIGN KEY (thread_id) REFERENCES communication.communication_threads (thread_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_messages_sender_participant
        FOREIGN KEY (sender_participant_id) REFERENCES communication.communication_participants (participant_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_messages_recipient_participant
        FOREIGN KEY (recipient_participant_id) REFERENCES communication.communication_participants (participant_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_messages_sender_user
        FOREIGN KEY (sender_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_messages_sender_notary
        FOREIGN KEY (sender_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_messages_reply_to
        FOREIGN KEY (reply_to_message_id) REFERENCES communication.communication_messages (message_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_messages_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_messages_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_messages_tenant_code
        UNIQUE (tenant_id, message_code)
);

CREATE INDEX IF NOT EXISTS ix_messages_tenant_id
    ON communication.communication_messages (tenant_id);

CREATE INDEX IF NOT EXISTS ix_messages_thread_id
    ON communication.communication_messages (thread_id);

CREATE INDEX IF NOT EXISTS ix_messages_status
    ON communication.communication_messages (message_status);

CREATE INDEX IF NOT EXISTS ix_messages_direction
    ON communication.communication_messages (direction);

CREATE INDEX IF NOT EXISTS ix_messages_sent_at
    ON communication.communication_messages (sent_at);

CREATE INDEX IF NOT EXISTS ix_messages_linked_entity
    ON communication.communication_messages (linked_entity_type, linked_entity_id);

DROP TRIGGER IF EXISTS trg_messages_updated_at ON communication.communication_messages;
CREATE TRIGGER trg_messages_updated_at
BEFORE UPDATE ON communication.communication_messages
FOR EACH ROW EXECUTE FUNCTION communication.set_updated_at();

-- =========================================================
-- ATTACHMENTS
-- =========================================================

CREATE TABLE IF NOT EXISTS communication.communication_attachments (
    attachment_id           uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    message_id              uuid,
    thread_id               uuid NOT NULL,

    attachment_code         varchar(50) NOT NULL,
    file_name               varchar(255) NOT NULL,
    file_extension          varchar(20),
    mime_type               varchar(100),
    storage_provider        varchar(50) NOT NULL DEFAULT 'object_storage',
    storage_key             text NOT NULL,
    file_size_bytes         bigint,
    checksum_sha256         char(64),

    title                   varchar(300),
    description             text,
    is_sensitive            boolean NOT NULL DEFAULT false,
    visibility_level        varchar(50) NOT NULL DEFAULT 'restricted',

    uploaded_by_user_id     uuid,
    uploaded_at             timestamptz NOT NULL DEFAULT now(),
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_attachments_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_attachments_message
        FOREIGN KEY (message_id) REFERENCES communication.communication_messages (message_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_attachments_thread
        FOREIGN KEY (thread_id) REFERENCES communication.communication_threads (thread_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_attachments_uploaded_by
        FOREIGN KEY (uploaded_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_attachments_tenant_code
        UNIQUE (tenant_id, attachment_code),

    CONSTRAINT chk_attachments_size
        CHECK (file_size_bytes IS NULL OR file_size_bytes >= 0)
);

CREATE INDEX IF NOT EXISTS ix_attachments_tenant_id
    ON communication.communication_attachments (tenant_id);

CREATE INDEX IF NOT EXISTS ix_attachments_message_id
    ON communication.communication_attachments (message_id);

CREATE INDEX IF NOT EXISTS ix_attachments_thread_id
    ON communication.communication_attachments (thread_id);

CREATE INDEX IF NOT EXISTS ix_attachments_uploaded_at
    ON communication.communication_attachments (uploaded_at);

DROP TRIGGER IF EXISTS trg_attachments_updated_at ON communication.communication_attachments;
CREATE TRIGGER trg_attachments_updated_at
BEFORE UPDATE ON communication.communication_attachments
FOR EACH ROW EXECUTE FUNCTION communication.set_updated_at();

-- =========================================================
-- TEMPLATES
-- =========================================================

CREATE TABLE IF NOT EXISTS communication.communication_templates (
    template_id             uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    template_code           varchar(50) NOT NULL,
    template_name           varchar(200) NOT NULL,
    template_type           communication.template_type NOT NULL DEFAULT 'email',
    channel_type            communication.channel_type NOT NULL DEFAULT 'email',

    subject_template        varchar(300),
    body_text_template      text,
    body_html_template      text,
    is_system               boolean NOT NULL DEFAULT false,
    is_active               boolean NOT NULL DEFAULT true,

    variables_schema        jsonb NOT NULL DEFAULT '[]'::jsonb,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_templates_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_templates_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_templates_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_templates_tenant_code
        UNIQUE (tenant_id, template_code)
);

CREATE INDEX IF NOT EXISTS ix_templates_tenant_id
    ON communication.communication_templates (tenant_id);

CREATE INDEX IF NOT EXISTS ix_templates_template_type
    ON communication.communication_templates (template_type);

CREATE INDEX IF NOT EXISTS ix_templates_channel_type
    ON communication.communication_templates (channel_type);

CREATE INDEX IF NOT EXISTS ix_templates_is_active
    ON communication.communication_templates (is_active);

DROP TRIGGER IF EXISTS trg_templates_updated_at ON communication.communication_templates;
CREATE TRIGGER trg_templates_updated_at
BEFORE UPDATE ON communication.communication_templates
FOR EACH ROW EXECUTE FUNCTION communication.set_updated_at();

-- =========================================================
-- REMINDERS
-- =========================================================

CREATE TABLE IF NOT EXISTS communication.communication_reminders (
    reminder_id             uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    thread_id               uuid,
    message_id              uuid,
    job_id                  uuid,
    notarial_act_id         uuid,
    journal_entry_id        uuid,
    invoice_id              uuid,

    reminder_code           varchar(50) NOT NULL,
    reminder_status         communication.reminder_status NOT NULL DEFAULT 'pending',
    channel_type            communication.channel_type NOT NULL DEFAULT 'email',

    recipient_participant_id uuid,
    recipient_user_id       uuid,
    recipient_notary_id     uuid,
    recipient_email        citext,
    recipient_phone        varchar(30),

    template_id             uuid,
    title                   varchar(200),
    body_text               text,
    payload                 jsonb NOT NULL DEFAULT '{}'::jsonb,

    scheduled_at            timestamptz NOT NULL,
    sent_at                 timestamptz,
    delivered_at            timestamptz,
    failed_at               timestamptz,
    snoozed_until           timestamptz,
    failure_reason          text,

    created_by_user_id      uuid,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_reminders_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_reminders_thread
        FOREIGN KEY (thread_id) REFERENCES communication.communication_threads (thread_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_reminders_message
        FOREIGN KEY (message_id) REFERENCES communication.communication_messages (message_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_reminders_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_reminders_notarial_act
        FOREIGN KEY (notarial_act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_reminders_journal_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_reminders_invoice
        FOREIGN KEY (invoice_id) REFERENCES billing.invoices (invoice_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_reminders_recipient_participant
        FOREIGN KEY (recipient_participant_id) REFERENCES communication.communication_participants (participant_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_reminders_recipient_user
        FOREIGN KEY (recipient_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_reminders_recipient_notary
        FOREIGN KEY (recipient_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_reminders_template
        FOREIGN KEY (template_id) REFERENCES communication.communication_templates (template_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_reminders_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_reminders_tenant_code
        UNIQUE (tenant_id, reminder_code)
);

CREATE INDEX IF NOT EXISTS ix_reminders_tenant_id
    ON communication.communication_reminders (tenant_id);

CREATE INDEX IF NOT EXISTS ix_reminders_status
    ON communication.communication_reminders (reminder_status);

CREATE INDEX IF NOT EXISTS ix_reminders_channel_type
    ON communication.communication_reminders (channel_type);

CREATE INDEX IF NOT EXISTS ix_reminders_scheduled_at
    ON communication.communication_reminders (scheduled_at);

DROP TRIGGER IF EXISTS trg_reminders_updated_at ON communication.communication_reminders;
CREATE TRIGGER trg_reminders_updated_at
BEFORE UPDATE ON communication.communication_reminders
FOR EACH ROW EXECUTE FUNCTION communication.set_updated_at();

-- =========================================================
-- DELIVERY LOGS
-- =========================================================

CREATE TABLE IF NOT EXISTS communication.communication_delivery_logs (
    delivery_log_id         uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    reminder_id             uuid,
    message_id              uuid,
    thread_id               uuid NOT NULL,

    delivery_code           varchar(50) NOT NULL,
    channel_type            communication.channel_type NOT NULL,
    delivery_status         communication.delivery_status NOT NULL DEFAULT 'pending',

    provider_name           varchar(200),
    provider_message_id     text,
    recipient_address      text,

    requested_at            timestamptz NOT NULL DEFAULT now(),
    queued_at               timestamptz,
    sent_at                 timestamptz,
    delivered_at            timestamptz,
    failed_at               timestamptz,
    bounced_at              timestamptz,

    failure_reason          text,
    response_payload        jsonb NOT NULL DEFAULT '{}'::jsonb,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_delivery_logs_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_delivery_logs_reminder
        FOREIGN KEY (reminder_id) REFERENCES communication.communication_reminders (reminder_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_delivery_logs_message
        FOREIGN KEY (message_id) REFERENCES communication.communication_messages (message_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_delivery_logs_thread
        FOREIGN KEY (thread_id) REFERENCES communication.communication_threads (thread_id)
        ON DELETE CASCADE,

    CONSTRAINT uq_delivery_logs_tenant_code
        UNIQUE (tenant_id, delivery_code)
);

CREATE INDEX IF NOT EXISTS ix_delivery_logs_tenant_id
    ON communication.communication_delivery_logs (tenant_id);

CREATE INDEX IF NOT EXISTS ix_delivery_logs_thread_id
    ON communication.communication_delivery_logs (thread_id);

CREATE INDEX IF NOT EXISTS ix_delivery_logs_status
    ON communication.communication_delivery_logs (delivery_status);

CREATE INDEX IF NOT EXISTS ix_delivery_logs_channel_type
    ON communication.communication_delivery_logs (channel_type);

CREATE INDEX IF NOT EXISTS ix_delivery_logs_requested_at
    ON communication.communication_delivery_logs (requested_at);

DROP TRIGGER IF EXISTS trg_delivery_logs_updated_at ON communication.communication_delivery_logs;
CREATE TRIGGER trg_delivery_logs_updated_at
BEFORE UPDATE ON communication.communication_delivery_logs
FOR EACH ROW EXECUTE FUNCTION communication.set_updated_at();

-- =========================================================
-- INTERNAL NOTES
-- =========================================================

CREATE TABLE IF NOT EXISTS communication.internal_notes (
    internal_note_id        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    thread_id               uuid,
    customer_id             uuid,
    customer_contact_id     uuid,
    job_id                  uuid,
    notarial_act_id         uuid,
    invoice_id              uuid,
    incident_id             uuid,

    note_code               varchar(50) NOT NULL,
    title                   varchar(200),
    body                    text NOT NULL,
    visibility              communication.note_visibility NOT NULL DEFAULT 'private',
    is_pinned               boolean NOT NULL DEFAULT false,
    is_compliance_note      boolean NOT NULL DEFAULT false,

    created_by_user_id      uuid NOT NULL,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_internal_notes_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_internal_notes_thread
        FOREIGN KEY (thread_id) REFERENCES communication.communication_threads (thread_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_internal_notes_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_internal_notes_customer_contact
        FOREIGN KEY (customer_contact_id) REFERENCES crm.customer_contacts (contact_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_internal_notes_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_internal_notes_notarial_act
        FOREIGN KEY (notarial_act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_internal_notes_invoice
        FOREIGN KEY (invoice_id) REFERENCES billing.invoices (invoice_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_internal_notes_incident
        FOREIGN KEY (incident_id) REFERENCES security.security_incidents (incident_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_internal_notes_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_internal_notes_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_internal_notes_tenant_code
        UNIQUE (tenant_id, note_code)
);

CREATE INDEX IF NOT EXISTS ix_internal_notes_tenant_id
    ON communication.internal_notes (tenant_id);

CREATE INDEX IF NOT EXISTS ix_internal_notes_thread_id
    ON communication.internal_notes (thread_id);

CREATE INDEX IF NOT EXISTS ix_internal_notes_customer_id
    ON communication.internal_notes (customer_id);

CREATE INDEX IF NOT EXISTS ix_internal_notes_job_id
    ON communication.internal_notes (job_id);

CREATE INDEX IF NOT EXISTS ix_internal_notes_visibility
    ON communication.internal_notes (visibility);

CREATE INDEX IF NOT EXISTS ix_internal_notes_created_at
    ON communication.internal_notes (created_at);

DROP TRIGGER IF EXISTS trg_internal_notes_updated_at ON communication.internal_notes;
CREATE TRIGGER trg_internal_notes_updated_at
BEFORE UPDATE ON communication.internal_notes
FOR EACH ROW EXECUTE FUNCTION communication.set_updated_at();

-- =========================================================
-- CALL LOGS
-- =========================================================

CREATE TABLE IF NOT EXISTS communication.call_logs (
    call_log_id             uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    thread_id               uuid,
    reminder_id             uuid,
    message_id              uuid,

    call_code               varchar(50) NOT NULL,
    call_status             communication.call_status NOT NULL DEFAULT 'scheduled',
    call_outcome            communication.call_outcome NOT NULL DEFAULT 'other',

    caller_user_id          uuid,
    caller_notary_id        uuid,
    callee_participant_id   uuid,
    callee_user_id          uuid,
    callee_notary_id        uuid,
    callee_phone            varchar(30),

    scheduled_at            timestamptz,
    started_at              timestamptz,
    ended_at                timestamptz,
    duration_seconds        integer,

    summary                 text,
    notes                   text,
    recording_file_name     varchar(255),
    recording_storage_key   text,
    recording_mime_type     varchar(100),
    recording_file_size_bytes bigint,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_call_logs_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_call_logs_thread
        FOREIGN KEY (thread_id) REFERENCES communication.communication_threads (thread_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_call_logs_reminder
        FOREIGN KEY (reminder_id) REFERENCES communication.communication_reminders (reminder_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_call_logs_message
        FOREIGN KEY (message_id) REFERENCES communication.communication_messages (message_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_call_logs_caller_user
        FOREIGN KEY (caller_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_call_logs_caller_notary
        FOREIGN KEY (caller_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_call_logs_callee_participant
        FOREIGN KEY (callee_participant_id) REFERENCES communication.communication_participants (participant_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_call_logs_callee_user
        FOREIGN KEY (callee_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_call_logs_callee_notary
        FOREIGN KEY (callee_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_call_logs_tenant_code
        UNIQUE (tenant_id, call_code),

    CONSTRAINT chk_call_logs_duration
        CHECK (duration_seconds IS NULL OR duration_seconds >= 0),

    CONSTRAINT chk_call_logs_recording_size
        CHECK (recording_file_size_bytes IS NULL OR recording_file_size_bytes >= 0)
);

CREATE INDEX IF NOT EXISTS ix_call_logs_tenant_id
    ON communication.call_logs (tenant_id);

CREATE INDEX IF NOT EXISTS ix_call_logs_thread_id
    ON communication.call_logs (thread_id);

CREATE INDEX IF NOT EXISTS ix_call_logs_status
    ON communication.call_logs (call_status);

CREATE INDEX IF NOT EXISTS ix_call_logs_outcome
    ON communication.call_logs (call_outcome);

CREATE INDEX IF NOT EXISTS ix_call_logs_started_at
    ON communication.call_logs (started_at);

DROP TRIGGER IF EXISTS trg_call_logs_updated_at ON communication.call_logs;
CREATE TRIGGER trg_call_logs_updated_at
BEFORE UPDATE ON communication.call_logs
FOR EACH ROW EXECUTE FUNCTION communication.set_updated_at();

-- =========================================================
-- AUDIT LOGS
-- =========================================================

CREATE TABLE IF NOT EXISTS communication.communication_audit_logs (
    communication_audit_log_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    audit_code              varchar(50) NOT NULL,
    event_type              communication.audit_event_type NOT NULL,
    entity_type             varchar(100) NOT NULL,
    entity_id               uuid,
    entity_code             varchar(100),

    actor_user_id           uuid,
    actor_notary_id         uuid,
    source_ip               inet,
    user_agent              text,

    before_data             jsonb,
    after_data              jsonb,
    occurred_at             timestamptz NOT NULL DEFAULT now(),
    source_reference        text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_communication_audit_logs_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_communication_audit_logs_actor_user
        FOREIGN KEY (actor_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_communication_audit_logs_actor_notary
        FOREIGN KEY (actor_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_communication_audit_logs_tenant_code
        UNIQUE (tenant_id, audit_code)
);

CREATE INDEX IF NOT EXISTS ix_communication_audit_logs_tenant_id
    ON communication.communication_audit_logs (tenant_id);

CREATE INDEX IF NOT EXISTS ix_communication_audit_logs_event_type
    ON communication.communication_audit_logs (event_type);

CREATE INDEX IF NOT EXISTS ix_communication_audit_logs_entity
    ON communication.communication_audit_logs (entity_type, entity_id);

CREATE INDEX IF NOT EXISTS ix_communication_audit_logs_occurred_at
    ON communication.communication_audit_logs (occurred_at);

-- =========================================================
-- VIEWS
-- =========================================================

CREATE OR REPLACE VIEW communication.v_thread_overview AS
SELECT
    t.thread_id,
    t.tenant_id,
    t.thread_code,
    t.channel_type,
    t.thread_status,
    t.subject,
    t.is_internal,
    t.is_important,
    t.customer_id,
    c.display_name AS customer_name,
    t.job_id,
    t.notarial_act_id,
    t.invoice_id,
    t.incident_id,
    t.last_message_at,
    t.last_activity_at,
    t.closed_at,
    t.archived_at
FROM communication.communication_threads t
LEFT JOIN crm.customers c
    ON c.customer_id = t.customer_id
WHERE t.deleted_at IS NULL
  AND (c.deleted_at IS NULL OR c.customer_id IS NULL);

CREATE OR REPLACE VIEW communication.v_latest_message_per_thread AS
SELECT DISTINCT ON (m.thread_id)
    m.message_id,
    m.tenant_id,
    m.thread_id,
    m.message_code,
    m.direction,
    m.message_status,
    m.subject,
    m.sent_at,
    m.delivered_at,
    m.read_at,
    m.is_important,
    m.is_internal
FROM communication.communication_messages m
WHERE m.deleted_at IS NULL
ORDER BY m.thread_id, m.created_at DESC;

CREATE OR REPLACE VIEW communication.v_pending_reminders AS
SELECT
    r.reminder_id,
    r.tenant_id,
    r.reminder_code,
    r.reminder_status,
    r.channel_type,
    r.scheduled_at,
    r.job_id,
    r.notarial_act_id,
    r.journal_entry_id,
    r.invoice_id
FROM communication.communication_reminders r
WHERE r.deleted_at IS NULL
  AND r.reminder_status IN ('pending', 'queued', 'snoozed');

-- =========================================================
-- COMMENTS
-- =========================================================

COMMENT ON SCHEMA communication IS 'Communication schema for interaction timelines, messages, participants, attachments, reminders, delivery tracking, notes, calls, and audit logs.';
COMMENT ON TABLE communication.communication_threads IS 'Conversation/thread aggregate tied to customer, job, act, invoice, or incident.';
COMMENT ON TABLE communication.communication_participants IS 'Participants in a communication thread.';
COMMENT ON TABLE communication.communication_messages IS 'Individual inbound/outbound/internal messages.';
COMMENT ON TABLE communication.communication_attachments IS 'Files attached to messages or threads.';
COMMENT ON TABLE communication.communication_templates IS 'Reusable templates for email, SMS, call scripts, and meetings.';
COMMENT ON TABLE communication.communication_reminders IS 'Scheduled reminders and notifications.';
COMMENT ON TABLE communication.communication_delivery_logs IS 'Delivery attempts and provider responses.';
COMMENT ON TABLE communication.internal_notes IS 'Internal notes for sales, operations, and compliance.';
COMMENT ON TABLE communication.call_logs IS 'Call records, outcomes, and optional recordings.';
COMMENT ON TABLE communication.communication_audit_logs IS 'Evidence-grade audit trail for communication operations.';


-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================


-- PostgreSQL 15+
-- Compliance schema for rules, checks, policy versions, legal holds,
-- audit trail, retention, regulatory exports, inspections, and incidents.

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

CREATE SCHEMA IF NOT EXISTS compliance;

-- =========================================================
-- ENUM TYPES
-- =========================================================

DO $$
BEGIN
    IF to_regtype('compliance.rule_status') IS NULL THEN
        CREATE TYPE compliance.rule_status AS ENUM (
            'draft',
            'active',
            'inactive',
            'archived'
        );
    END IF;

    IF to_regtype('compliance.rule_severity') IS NULL THEN
        CREATE TYPE compliance.rule_severity AS ENUM (
            'low',
            'medium',
            'high',
            'critical'
        );
    END IF;

    IF to_regtype('compliance.rule_scope') IS NULL THEN
        CREATE TYPE compliance.rule_scope AS ENUM (
            'tenant',
            'branch',
            'region',
            'state',
            'notary',
            'role',
            'customer',
            'service_type',
            'job',
            'act',
            'journal_entry',
            'seal',
            'certificate'
        );
    END IF;

    IF to_regtype('compliance.check_status') IS NULL THEN
        CREATE TYPE compliance.check_status AS ENUM (
            'pending',
            'running',
            'passed',
            'failed',
            'warning',
            'blocked',
            'skipped',
            'manual_review'
        );
    END IF;

    IF to_regtype('compliance.check_result_severity') IS NULL THEN
        CREATE TYPE compliance.check_result_severity AS ENUM (
            'info',
            'warning',
            'error',
            'critical'
        );
    END IF;

    IF to_regtype('compliance.policy_status') IS NULL THEN
        CREATE TYPE compliance.policy_status AS ENUM (
            'draft',
            'active',
            'inactive',
            'archived'
        );
    END IF;

    IF to_regtype('compliance.legal_hold_status') IS NULL THEN
        CREATE TYPE compliance.legal_hold_status AS ENUM (
            'active',
            'released',
            'expired',
            'cancelled'
        );
    END IF;

    IF to_regtype('compliance.audit_event_type') IS NULL THEN
        CREATE TYPE compliance.audit_event_type AS ENUM (
            'create',
            'update',
            'delete',
            'status_change',
            'rule_evaluated',
            'check_started',
            'check_completed',
            'check_failed',
            'blocked',
            'unblocked',
            'hold_applied',
            'hold_released',
            'policy_versioned',
            'retention_applied',
            'retention_scheduled',
            'export_requested',
            'export_generated',
            'inspection_started',
            'inspection_completed',
            'incident_opened',
            'incident_updated',
            'incident_closed'
        );
    END IF;

    IF to_regtype('compliance.export_status') IS NULL THEN
        CREATE TYPE compliance.export_status AS ENUM (
            'queued',
            'processing',
            'generated',
            'failed',
            'downloaded',
            'expired'
        );
    END IF;

    IF to_regtype('compliance.export_format') IS NULL THEN
        CREATE TYPE compliance.export_format AS ENUM (
            'pdf',
            'csv',
            'xlsx',
            'json',
            'zip'
        );
    END IF;

    IF to_regtype('compliance.inspection_status') IS NULL THEN
        CREATE TYPE compliance.inspection_status AS ENUM (
            'planned',
            'in_progress',
            'awaiting_response',
            'completed',
            'closed',
            'cancelled'
        );
    END IF;

    IF to_regtype('compliance.incident_status') IS NULL THEN
        CREATE TYPE compliance.incident_status AS ENUM (
            'open',
            'investigating',
            'contained',
            'resolved',
            'closed',
            'escalated'
        );
    END IF;

    IF to_regtype('compliance.incident_severity') IS NULL THEN
        CREATE TYPE compliance.incident_severity AS ENUM (
            'low',
            'medium',
            'high',
            'critical'
        );
    END IF;

    IF to_regtype('compliance.incident_type') IS NULL THEN
        CREATE TYPE compliance.incident_type AS ENUM (
            'policy_violation',
            'state_rule_violation',
            'unauthorized_access',
            'missing_journal_entry',
            'expired_seal_usage',
            'expired_certificate_usage',
            'late_filing',
            'data_retention_violation',
            'privacy_violation',
            'other'
        );
    END IF;
END $$;

-- =========================================================
-- UPDATED_AT TRIGGER
-- =========================================================

CREATE OR REPLACE FUNCTION compliance.set_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at = now();
    RETURN NEW;
END;
$$;

-- =========================================================
-- COMPLIANCE RULES
-- Central rule catalog for state/company/branch/etc.
-- =========================================================

CREATE TABLE IF NOT EXISTS compliance.compliance_rules (
    compliance_rule_id      uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    rule_code               varchar(50)  NOT NULL,
    rule_name               varchar(200) NOT NULL,
    rule_status             compliance.rule_status NOT NULL DEFAULT 'draft',
    severity                compliance.rule_severity NOT NULL DEFAULT 'medium',
    scope                   compliance.rule_scope NOT NULL DEFAULT 'tenant',

    state_code              char(2),
    branch_id               uuid,
    region_id               uuid,
    notary_id               uuid,
    role_id                 uuid,
    customer_id             uuid,
    service_type_id         uuid,
    job_id                  uuid,
    act_id                  uuid,
    journal_entry_id        uuid,
    seal_id                 uuid,
    certificate_id          uuid,

    category                varchar(100),
    subcategory             varchar(100),
    title                   varchar(250),
    description             text,
    rationale               text,

    condition_expression    text NOT NULL,
    action_expression       text,
    block_on_failure        boolean NOT NULL DEFAULT true,
    requires_manual_review  boolean NOT NULL DEFAULT false,
    is_mandatory            boolean NOT NULL DEFAULT false,
    is_active               boolean NOT NULL DEFAULT true,

    effective_from          timestamptz,
    effective_to            timestamptz,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_compliance_rules_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_compliance_rules_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_rules_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_rules_notary
        FOREIGN KEY (notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_rules_role
        FOREIGN KEY (role_id) REFERENCES core.roles (role_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_rules_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_rules_service_type
        FOREIGN KEY (service_type_id) REFERENCES operations.service_types (service_type_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_rules_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_rules_act
        FOREIGN KEY (act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_rules_journal_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_rules_seal
        FOREIGN KEY (seal_id) REFERENCES security.seals (seal_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_rules_certificate
        FOREIGN KEY (certificate_id) REFERENCES security.digital_certificates (digital_certificate_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_rules_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_rules_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_compliance_rules_tenant_code
        UNIQUE (tenant_id, rule_code),

    CONSTRAINT chk_compliance_rules_effective_dates
        CHECK (effective_to IS NULL OR effective_from IS NULL OR effective_to >= effective_from)
);

CREATE INDEX IF NOT EXISTS ix_compliance_rules_tenant_id
    ON compliance.compliance_rules (tenant_id);

CREATE INDEX IF NOT EXISTS ix_compliance_rules_status
    ON compliance.compliance_rules (rule_status);

CREATE INDEX IF NOT EXISTS ix_compliance_rules_severity
    ON compliance.compliance_rules (severity);

CREATE INDEX IF NOT EXISTS ix_compliance_rules_scope
    ON compliance.compliance_rules (scope);

CREATE INDEX IF NOT EXISTS ix_compliance_rules_state_code
    ON compliance.compliance_rules (state_code);

CREATE INDEX IF NOT EXISTS ix_compliance_rules_is_active
    ON compliance.compliance_rules (is_active);

DROP TRIGGER IF EXISTS trg_compliance_rules_updated_at ON compliance.compliance_rules;
CREATE TRIGGER trg_compliance_rules_updated_at
BEFORE UPDATE ON compliance.compliance_rules
FOR EACH ROW EXECUTE FUNCTION compliance.set_updated_at();

-- =========================================================
-- POLICY VERSIONS
-- Version history for policies and rule bundles
-- =========================================================

CREATE TABLE IF NOT EXISTS compliance.policy_versions (
    policy_version_id       uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    policy_code             varchar(50)  NOT NULL,
    policy_name             varchar(200) NOT NULL,
    policy_status           compliance.policy_status NOT NULL DEFAULT 'draft',

    version_no              integer NOT NULL DEFAULT 1,
    parent_policy_version_id uuid,

    state_code              char(2),
    branch_id               uuid,
    region_id               uuid,
    scope                   compliance.rule_scope NOT NULL DEFAULT 'tenant',

    effective_from          timestamptz,
    effective_to            timestamptz,
    published_at            timestamptz,
    deprecated_at           timestamptz,

    change_summary          text,
    policy_body             text NOT NULL,
    policy_json             jsonb NOT NULL DEFAULT '{}'::jsonb,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_policy_versions_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_policy_versions_parent
        FOREIGN KEY (parent_policy_version_id) REFERENCES compliance.policy_versions (policy_version_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_policy_versions_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_policy_versions_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_policy_versions_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_policy_versions_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_policy_versions_tenant_code_version
        UNIQUE (tenant_id, policy_code, version_no),

    CONSTRAINT chk_policy_versions_version_no
        CHECK (version_no >= 1),

    CONSTRAINT chk_policy_versions_effective_dates
        CHECK (effective_to IS NULL OR effective_from IS NULL OR effective_to >= effective_from)
);

CREATE INDEX IF NOT EXISTS ix_policy_versions_tenant_id
    ON compliance.policy_versions (tenant_id);

CREATE INDEX IF NOT EXISTS ix_policy_versions_status
    ON compliance.policy_versions (policy_status);

CREATE INDEX IF NOT EXISTS ix_policy_versions_scope
    ON compliance.policy_versions (scope);

CREATE INDEX IF NOT EXISTS ix_policy_versions_state_code
    ON compliance.policy_versions (state_code);

CREATE INDEX IF NOT EXISTS ix_policy_versions_published_at
    ON compliance.policy_versions (published_at);

DROP TRIGGER IF EXISTS trg_policy_versions_updated_at ON compliance.policy_versions;
CREATE TRIGGER trg_policy_versions_updated_at
BEFORE UPDATE ON compliance.policy_versions
FOR EACH ROW EXECUTE FUNCTION compliance.set_updated_at();

-- =========================================================
-- COMPLIANCE CHECKS
-- Run instances of compliance evaluation
-- =========================================================

CREATE TABLE IF NOT EXISTS compliance.compliance_checks (
    compliance_check_id     uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    check_code              varchar(50)  NOT NULL,
    check_name              varchar(200) NOT NULL,
    check_status            compliance.check_status NOT NULL DEFAULT 'pending',

    rule_bundle_code        varchar(50),
    policy_version_id       uuid,
    triggered_by_user_id   uuid,
    triggered_by_notary_id uuid,

    state_code              char(2),
    branch_id               uuid,
    region_id               uuid,
    customer_id             uuid,
    service_type_id         uuid,
    job_id                  uuid,
    act_id                  uuid,
    journal_entry_id        uuid,
    seal_id                 uuid,
    certificate_id          uuid,

    started_at              timestamptz,
    completed_at            timestamptz,
    blocked_at              timestamptz,
    manual_review_at        timestamptz,

    block_reason            text,
    summary                 text,
    result_summary          jsonb NOT NULL DEFAULT '{}'::jsonb,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_compliance_checks_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_compliance_checks_policy_version
        FOREIGN KEY (policy_version_id) REFERENCES compliance.policy_versions (policy_version_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_checks_triggered_by_user
        FOREIGN KEY (triggered_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_checks_triggered_by_notary
        FOREIGN KEY (triggered_by_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_checks_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_checks_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_checks_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_checks_service_type
        FOREIGN KEY (service_type_id) REFERENCES operations.service_types (service_type_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_checks_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_checks_act
        FOREIGN KEY (act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_checks_journal_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_checks_seal
        FOREIGN KEY (seal_id) REFERENCES security.seals (seal_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_checks_certificate
        FOREIGN KEY (certificate_id) REFERENCES security.digital_certificates (digital_certificate_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_compliance_checks_tenant_code
        UNIQUE (tenant_id, check_code)
);

CREATE INDEX IF NOT EXISTS ix_compliance_checks_tenant_id
    ON compliance.compliance_checks (tenant_id);

CREATE INDEX IF NOT EXISTS ix_compliance_checks_status
    ON compliance.compliance_checks (check_status);

CREATE INDEX IF NOT EXISTS ix_compliance_checks_state_code
    ON compliance.compliance_checks (state_code);

CREATE INDEX IF NOT EXISTS ix_compliance_checks_policy_version_id
    ON compliance.compliance_checks (policy_version_id);

CREATE INDEX IF NOT EXISTS ix_compliance_checks_started_at
    ON compliance.compliance_checks (started_at);

DROP TRIGGER IF EXISTS trg_compliance_checks_updated_at ON compliance.compliance_checks;
CREATE TRIGGER trg_compliance_checks_updated_at
BEFORE UPDATE ON compliance.compliance_checks
FOR EACH ROW EXECUTE FUNCTION compliance.set_updated_at();

-- =========================================================
-- COMPLIANCE CHECK RESULTS
-- Individual findings for each check
-- =========================================================

CREATE TABLE IF NOT EXISTS compliance.compliance_check_results (
    compliance_check_result_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    compliance_check_id     uuid NOT NULL,

    rule_code               varchar(50),
    rule_name               varchar(200),
    result_status           compliance.check_status NOT NULL DEFAULT 'pending',
    severity                compliance.check_result_severity NOT NULL DEFAULT 'info',

    entity_type             varchar(100),
    entity_id               uuid,
    field_name              varchar(100),
    expected_value          text,
    actual_value            text,
    message                 text,
    recommendation          text,

    is_blocking             boolean NOT NULL DEFAULT false,
    requires_manual_review  boolean NOT NULL DEFAULT false,

    evaluated_at            timestamptz NOT NULL DEFAULT now(),
    evaluated_by_user_id    uuid,
    evaluated_by_notary_id  uuid,

    evidence                jsonb NOT NULL DEFAULT '{}'::jsonb,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_compliance_check_results_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_compliance_check_results_check
        FOREIGN KEY (compliance_check_id) REFERENCES compliance.compliance_checks (compliance_check_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_compliance_check_results_evaluated_by_user
        FOREIGN KEY (evaluated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_check_results_evaluated_by_notary
        FOREIGN KEY (evaluated_by_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_compliance_check_results_tenant_id
    ON compliance.compliance_check_results (tenant_id);

CREATE INDEX IF NOT EXISTS ix_compliance_check_results_check_id
    ON compliance.compliance_check_results (compliance_check_id);

CREATE INDEX IF NOT EXISTS ix_compliance_check_results_status
    ON compliance.compliance_check_results (result_status);

CREATE INDEX IF NOT EXISTS ix_compliance_check_results_severity
    ON compliance.compliance_check_results (severity);

CREATE INDEX IF NOT EXISTS ix_compliance_check_results_entity
    ON compliance.compliance_check_results (entity_type, entity_id);

CREATE INDEX IF NOT EXISTS ix_compliance_check_results_evaluated_at
    ON compliance.compliance_check_results (evaluated_at);

DROP TRIGGER IF EXISTS trg_compliance_check_results_updated_at ON compliance.compliance_check_results;
CREATE TRIGGER trg_compliance_check_results_updated_at
BEFORE UPDATE ON compliance.compliance_check_results
FOR EACH ROW EXECUTE FUNCTION compliance.set_updated_at();

-- =========================================================
-- LEGAL HOLDS
-- =========================================================

CREATE TABLE IF NOT EXISTS compliance.legal_holds (
    legal_hold_id           uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    hold_code               varchar(50)  NOT NULL,
    hold_name               varchar(200) NOT NULL,
    hold_status             compliance.legal_hold_status NOT NULL DEFAULT 'active',

    hold_scope              compliance.rule_scope NOT NULL DEFAULT 'tenant',
    state_code              char(2),
    branch_id               uuid,
    region_id               uuid,
    customer_id             uuid,
    job_id                  uuid,
    act_id                  uuid,
    journal_entry_id        uuid,
    seal_id                 uuid,
    certificate_id          uuid,
    incident_id             uuid,

    reason                  text NOT NULL,
    hold_details            text,
    applied_at              timestamptz NOT NULL DEFAULT now(),
    released_at             timestamptz,
    expires_at              timestamptz,
    release_reason          text,

    applied_by_user_id      uuid NOT NULL,
    released_by_user_id     uuid,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_legal_holds_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_legal_holds_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_legal_holds_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_legal_holds_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_legal_holds_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_legal_holds_act
        FOREIGN KEY (act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_legal_holds_journal_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_legal_holds_seal
        FOREIGN KEY (seal_id) REFERENCES security.seals (seal_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_legal_holds_certificate
        FOREIGN KEY (certificate_id) REFERENCES security.digital_certificates (digital_certificate_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_legal_holds_incident
        FOREIGN KEY (incident_id) REFERENCES security.security_incidents (incident_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_legal_holds_applied_by
        FOREIGN KEY (applied_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_legal_holds_released_by
        FOREIGN KEY (released_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_legal_holds_tenant_code
        UNIQUE (tenant_id, hold_code),

    CONSTRAINT chk_legal_holds_dates
        CHECK (expires_at IS NULL OR released_at IS NULL OR expires_at >= released_at)
);

CREATE INDEX IF NOT EXISTS ix_legal_holds_tenant_id
    ON compliance.legal_holds (tenant_id);

CREATE INDEX IF NOT EXISTS ix_legal_holds_status
    ON compliance.legal_holds (hold_status);

CREATE INDEX IF NOT EXISTS ix_legal_holds_scope
    ON compliance.legal_holds (hold_scope);

CREATE INDEX IF NOT EXISTS ix_legal_holds_state_code
    ON compliance.legal_holds (state_code);

CREATE INDEX IF NOT EXISTS ix_legal_holds_applied_at
    ON compliance.legal_holds (applied_at);

DROP TRIGGER IF EXISTS trg_legal_holds_updated_at ON compliance.legal_holds;
CREATE TRIGGER trg_legal_holds_updated_at
BEFORE UPDATE ON compliance.legal_holds
FOR EACH ROW EXECUTE FUNCTION compliance.set_updated_at();

-- =========================================================
-- AUDIT EVENTS
-- =========================================================

CREATE TABLE IF NOT EXISTS compliance.compliance_audit_events (
    compliance_audit_event_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    audit_code              varchar(50) NOT NULL,
    event_type              compliance.audit_event_type NOT NULL,
    entity_type             varchar(100) NOT NULL,
    entity_id               uuid,
    entity_code             varchar(100),

    actor_user_id           uuid,
    actor_notary_id         uuid,
    source_ip               inet,
    user_agent              text,

    before_data             jsonb,
    after_data              jsonb,
    occurred_at             timestamptz NOT NULL DEFAULT now(),
    source_reference        text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_compliance_audit_events_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_compliance_audit_events_actor_user
        FOREIGN KEY (actor_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_audit_events_actor_notary
        FOREIGN KEY (actor_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_compliance_audit_events_tenant_code
        UNIQUE (tenant_id, audit_code)
);

CREATE INDEX IF NOT EXISTS ix_compliance_audit_events_tenant_id
    ON compliance.compliance_audit_events (tenant_id);

CREATE INDEX IF NOT EXISTS ix_compliance_audit_events_event_type
    ON compliance.compliance_audit_events (event_type);

CREATE INDEX IF NOT EXISTS ix_compliance_audit_events_entity
    ON compliance.compliance_audit_events (entity_type, entity_id);

CREATE INDEX IF NOT EXISTS ix_compliance_audit_events_occurred_at
    ON compliance.compliance_audit_events (occurred_at);

-- =========================================================
-- RETENTION POLICIES
-- =========================================================

CREATE TABLE IF NOT EXISTS compliance.retention_policies (
    retention_policy_id     uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    policy_code             varchar(50)  NOT NULL,
    policy_name             varchar(200) NOT NULL,
    policy_status           compliance.policy_status NOT NULL DEFAULT 'draft',

    state_code              char(2),
    scope                   compliance.rule_scope NOT NULL DEFAULT 'tenant',

    applies_to_entity_type  varchar(100) NOT NULL,
    retention_years         integer NOT NULL,
    destroy_after_retention boolean NOT NULL DEFAULT false,
    legal_hold_eligible     boolean NOT NULL DEFAULT true,

    effective_from          date NOT NULL,
    effective_to            date,
    notes                   text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_retention_policies_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_retention_policies_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_retention_policies_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_retention_policies_tenant_code
        UNIQUE (tenant_id, policy_code),

    CONSTRAINT chk_retention_policies_years
        CHECK (retention_years >= 0),

    CONSTRAINT chk_retention_policies_dates
        CHECK (effective_to IS NULL OR effective_to > effective_from)
);

CREATE INDEX IF NOT EXISTS ix_retention_policies_tenant_id
    ON compliance.retention_policies (tenant_id);

CREATE INDEX IF NOT EXISTS ix_retention_policies_status
    ON compliance.retention_policies (policy_status);

CREATE INDEX IF NOT EXISTS ix_retention_policies_state_code
    ON compliance.retention_policies (state_code);

CREATE INDEX IF NOT EXISTS ix_retention_policies_entity_type
    ON compliance.retention_policies (applies_to_entity_type);

DROP TRIGGER IF EXISTS trg_retention_policies_updated_at ON compliance.retention_policies;
CREATE TRIGGER trg_retention_policies_updated_at
BEFORE UPDATE ON compliance.retention_policies
FOR EACH ROW EXECUTE FUNCTION compliance.set_updated_at();

-- =========================================================
-- RETENTION JOBS
-- =========================================================

CREATE TABLE IF NOT EXISTS compliance.retention_jobs (
    retention_job_id        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    retention_job_code      varchar(50) NOT NULL,
    retention_policy_id     uuid NOT NULL,
    entity_type             varchar(100) NOT NULL,
    entity_id               uuid NOT NULL,

    scheduled_at            timestamptz NOT NULL DEFAULT now(),
    executed_at             timestamptz,
    job_status              varchar(50) NOT NULL DEFAULT 'pending',
    outcome                 text,

    legal_hold_blocked      boolean NOT NULL DEFAULT false,
    blocked_reason          text,

    requested_by_user_id    uuid,
    executed_by_user_id     uuid,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_retention_jobs_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_retention_jobs_policy
        FOREIGN KEY (retention_policy_id) REFERENCES compliance.retention_policies (retention_policy_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_retention_jobs_requested_by
        FOREIGN KEY (requested_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_retention_jobs_executed_by
        FOREIGN KEY (executed_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_retention_jobs_tenant_code
        UNIQUE (tenant_id, retention_job_code)
);

CREATE INDEX IF NOT EXISTS ix_retention_jobs_tenant_id
    ON compliance.retention_jobs (tenant_id);

CREATE INDEX IF NOT EXISTS ix_retention_jobs_policy_id
    ON compliance.retention_jobs (retention_policy_id);

CREATE INDEX IF NOT EXISTS ix_retention_jobs_entity
    ON compliance.retention_jobs (entity_type, entity_id);

CREATE INDEX IF NOT EXISTS ix_retention_jobs_scheduled_at
    ON compliance.retention_jobs (scheduled_at);

DROP TRIGGER IF EXISTS trg_retention_jobs_updated_at ON compliance.retention_jobs;
CREATE TRIGGER trg_retention_jobs_updated_at
BEFORE UPDATE ON compliance.retention_jobs
FOR EACH ROW EXECUTE FUNCTION compliance.set_updated_at();

-- =========================================================
-- REGULATORY EXPORTS
-- =========================================================

CREATE TABLE IF NOT EXISTS compliance.regulatory_exports (
    regulatory_export_id    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    export_code             varchar(50) NOT NULL,
    export_status           compliance.export_status NOT NULL DEFAULT 'queued',
    export_format           compliance.export_format NOT NULL DEFAULT 'pdf',

    export_scope            text NOT NULL,
    reason                  text,
    requested_at            timestamptz NOT NULL DEFAULT now(),
    generated_at            timestamptz,
    downloaded_at           timestamptz,
    expires_at              timestamptz,

    requested_by_user_id    uuid NOT NULL,
    generated_by_user_id    uuid,
    file_name               varchar(255),
    mime_type               varchar(100),
    storage_provider        varchar(50) NOT NULL DEFAULT 'object_storage',
    storage_key             text,
    file_size_bytes         bigint,
    checksum_sha256         char(64),

    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_regulatory_exports_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_regulatory_exports_requested_by
        FOREIGN KEY (requested_by_user_id) REFERENCES core.users (user_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_regulatory_exports_generated_by
        FOREIGN KEY (generated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_regulatory_exports_tenant_code
        UNIQUE (tenant_id, export_code),

    CONSTRAINT chk_regulatory_exports_size
        CHECK (file_size_bytes IS NULL OR file_size_bytes >= 0)
);

CREATE INDEX IF NOT EXISTS ix_regulatory_exports_tenant_id
    ON compliance.regulatory_exports (tenant_id);

CREATE INDEX IF NOT EXISTS ix_regulatory_exports_status
    ON compliance.regulatory_exports (export_status);

CREATE INDEX IF NOT EXISTS ix_regulatory_exports_requested_at
    ON compliance.regulatory_exports (requested_at);

DROP TRIGGER IF EXISTS trg_regulatory_exports_updated_at ON compliance.regulatory_exports;
CREATE TRIGGER trg_regulatory_exports_updated_at
BEFORE UPDATE ON compliance.regulatory_exports
FOR EACH ROW EXECUTE FUNCTION compliance.set_updated_at();

-- =========================================================
-- INSPECTIONS
-- =========================================================

CREATE TABLE IF NOT EXISTS compliance.inspections (
    inspection_id           uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    inspection_code         varchar(50) NOT NULL,
    inspection_name         varchar(200) NOT NULL,
    inspection_status       compliance.inspection_status NOT NULL DEFAULT 'planned',

    state_code              char(2),
    authority_name          varchar(200),
    case_number             varchar(100),
    started_at              timestamptz,
    completed_at            timestamptz,
    due_at                  timestamptz,

    inspector_name          varchar(200),
    inspector_email         citext,
    inspector_phone         varchar(30),

    scope_summary           text,
    findings_summary        text,
    remediation_summary     text,
    outcome                 text,

    related_export_id       uuid,
    legal_hold_id           uuid,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_by_user_id      uuid,
    updated_by_user_id      uuid,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_inspections_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_inspections_export
        FOREIGN KEY (related_export_id) REFERENCES compliance.regulatory_exports (regulatory_export_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_inspections_legal_hold
        FOREIGN KEY (legal_hold_id) REFERENCES compliance.legal_holds (legal_hold_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_inspections_created_by
        FOREIGN KEY (created_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_inspections_updated_by
        FOREIGN KEY (updated_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_inspections_tenant_code
        UNIQUE (tenant_id, inspection_code)
);

CREATE INDEX IF NOT EXISTS ix_inspections_tenant_id
    ON compliance.inspections (tenant_id);

CREATE INDEX IF NOT EXISTS ix_inspections_status
    ON compliance.inspections (inspection_status);

CREATE INDEX IF NOT EXISTS ix_inspections_state_code
    ON compliance.inspections (state_code);

CREATE INDEX IF NOT EXISTS ix_inspections_started_at
    ON compliance.inspections (started_at);

DROP TRIGGER IF EXISTS trg_inspections_updated_at ON compliance.inspections;
CREATE TRIGGER trg_inspections_updated_at
BEFORE UPDATE ON compliance.inspections
FOR EACH ROW EXECUTE FUNCTION compliance.set_updated_at();

-- =========================================================
-- INCIDENTS
-- =========================================================

CREATE TABLE IF NOT EXISTS compliance.incidents (
    incident_id             uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    incident_code           varchar(50) NOT NULL,
    incident_type           compliance.incident_type NOT NULL,
    incident_status         compliance.incident_status NOT NULL DEFAULT 'open',
    severity                compliance.incident_severity NOT NULL DEFAULT 'medium',

    title                   varchar(250) NOT NULL,
    summary                 text,
    details                 text,

    state_code              char(2),
    branch_id               uuid,
    region_id               uuid,
    customer_id             uuid,
    job_id                  uuid,
    act_id                  uuid,
    journal_entry_id        uuid,
    seal_id                 uuid,
    certificate_id          uuid,

    detected_at             timestamptz NOT NULL DEFAULT now(),
    reported_at             timestamptz,
    contained_at            timestamptz,
    resolved_at             timestamptz,
    closed_at               timestamptz,

    reported_by_user_id     uuid,
    assigned_to_user_id     uuid,
    legal_hold_applied      boolean NOT NULL DEFAULT false,
    legal_hold_id           uuid,

    regulatory_notification_required boolean NOT NULL DEFAULT false,
    regulatory_notified_at  timestamptz,
    external_reference      text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_incidents_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_incidents_branch
        FOREIGN KEY (branch_id) REFERENCES core.branches (branch_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_incidents_region
        FOREIGN KEY (region_id) REFERENCES core.regions (region_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_incidents_customer
        FOREIGN KEY (customer_id) REFERENCES crm.customers (customer_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_incidents_job
        FOREIGN KEY (job_id) REFERENCES operations.jobs (job_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_incidents_act
        FOREIGN KEY (act_id) REFERENCES notarial.notarial_acts (act_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_incidents_journal_entry
        FOREIGN KEY (journal_entry_id) REFERENCES journal.journal_entries (journal_entry_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_incidents_seal
        FOREIGN KEY (seal_id) REFERENCES security.seals (seal_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_incidents_certificate
        FOREIGN KEY (certificate_id) REFERENCES security.digital_certificates (digital_certificate_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_incidents_reported_by
        FOREIGN KEY (reported_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_incidents_assigned_to
        FOREIGN KEY (assigned_to_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_incidents_legal_hold
        FOREIGN KEY (legal_hold_id) REFERENCES compliance.legal_holds (legal_hold_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_incidents_tenant_code
        UNIQUE (tenant_id, incident_code)
);

CREATE INDEX IF NOT EXISTS ix_incidents_tenant_id
    ON compliance.incidents (tenant_id);

CREATE INDEX IF NOT EXISTS ix_incidents_status
    ON compliance.incidents (incident_status);

CREATE INDEX IF NOT EXISTS ix_incidents_severity
    ON compliance.incidents (severity);

CREATE INDEX IF NOT EXISTS ix_incidents_type
    ON compliance.incidents (incident_type);

CREATE INDEX IF NOT EXISTS ix_incidents_detected_at
    ON compliance.incidents (detected_at);

DROP TRIGGER IF EXISTS trg_incidents_updated_at ON compliance.incidents;
CREATE TRIGGER trg_incidents_updated_at
BEFORE UPDATE ON compliance.incidents
FOR EACH ROW EXECUTE FUNCTION compliance.set_updated_at();

-- =========================================================
-- INCIDENT ACTIONS
-- =========================================================

CREATE TABLE IF NOT EXISTS compliance.incident_actions (
    incident_action_id      uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,
    incident_id             uuid NOT NULL,

    action_code             varchar(50) NOT NULL,
    action_title            varchar(200) NOT NULL,
    action_body             text,
    action_status           varchar(50) NOT NULL DEFAULT 'open',

    assigned_to_user_id     uuid,
    due_at                  timestamptz,
    completed_at            timestamptz,
    performed_by_user_id    uuid,

    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,
    created_at              timestamptz NOT NULL DEFAULT now(),
    updated_at              timestamptz NOT NULL DEFAULT now(),
    deleted_at              timestamptz,

    CONSTRAINT fk_incident_actions_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_incident_actions_incident
        FOREIGN KEY (incident_id) REFERENCES compliance.incidents (incident_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_incident_actions_assigned_to
        FOREIGN KEY (assigned_to_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_incident_actions_performed_by
        FOREIGN KEY (performed_by_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_incident_actions_incident_code
        UNIQUE (incident_id, action_code)
);

CREATE INDEX IF NOT EXISTS ix_incident_actions_tenant_id
    ON compliance.incident_actions (tenant_id);

CREATE INDEX IF NOT EXISTS ix_incident_actions_incident_id
    ON compliance.incident_actions (incident_id);

CREATE INDEX IF NOT EXISTS ix_incident_actions_due_at
    ON compliance.incident_actions (due_at);

DROP TRIGGER IF EXISTS trg_incident_actions_updated_at ON compliance.incident_actions;
CREATE TRIGGER trg_incident_actions_updated_at
BEFORE UPDATE ON compliance.incident_actions
FOR EACH ROW EXECUTE FUNCTION compliance.set_updated_at();

-- =========================================================
-- COMPLIANCE AUDIT LOGS
-- =========================================================

CREATE TABLE IF NOT EXISTS compliance.compliance_audit_logs (
    compliance_audit_log_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id               uuid NOT NULL,

    audit_code              varchar(50) NOT NULL,
    event_type              compliance.audit_event_type NOT NULL,
    entity_type             varchar(100) NOT NULL,
    entity_id               uuid,
    entity_code             varchar(100),

    actor_user_id           uuid,
    actor_notary_id         uuid,
    source_ip               inet,
    user_agent              text,

    before_data             jsonb,
    after_data              jsonb,
    occurred_at             timestamptz NOT NULL DEFAULT now(),
    source_reference        text,
    metadata                jsonb NOT NULL DEFAULT '{}'::jsonb,

    created_at              timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_compliance_audit_logs_tenant
        FOREIGN KEY (tenant_id) REFERENCES core.tenants (tenant_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_compliance_audit_logs_actor_user
        FOREIGN KEY (actor_user_id) REFERENCES core.users (user_id)
        ON DELETE SET NULL,

    CONSTRAINT fk_compliance_audit_logs_actor_notary
        FOREIGN KEY (actor_notary_id) REFERENCES identity.notaries (notary_id)
        ON DELETE SET NULL,

    CONSTRAINT uq_compliance_audit_logs_tenant_code
        UNIQUE (tenant_id, audit_code)
);

CREATE INDEX IF NOT EXISTS ix_compliance_audit_logs_tenant_id
    ON compliance.compliance_audit_logs (tenant_id);

CREATE INDEX IF NOT EXISTS ix_compliance_audit_logs_event_type
    ON compliance.compliance_audit_logs (event_type);

CREATE INDEX IF NOT EXISTS ix_compliance_audit_logs_entity
    ON compliance.compliance_audit_logs (entity_type, entity_id);

CREATE INDEX IF NOT EXISTS ix_compliance_audit_logs_occurred_at
    ON compliance.compliance_audit_logs (occurred_at);

-- =========================================================
-- VIEWS
-- =========================================================

CREATE OR REPLACE VIEW compliance.v_active_rules AS
SELECT
    r.compliance_rule_id,
    r.tenant_id,
    r.rule_code,
    r.rule_name,
    r.rule_status,
    r.severity,
    r.scope,
    r.state_code,
    r.block_on_failure,
    r.requires_manual_review,
    r.is_mandatory,
    r.is_active,
    r.effective_from,
    r.effective_to
FROM compliance.compliance_rules r
WHERE r.deleted_at IS NULL
  AND r.is_active = true;

CREATE OR REPLACE VIEW compliance.v_current_policy_versions AS
SELECT
    pv.policy_version_id,
    pv.tenant_id,
    pv.policy_code,
    pv.policy_name,
    pv.policy_status,
    pv.version_no,
    pv.state_code,
    pv.scope,
    pv.effective_from,
    pv.effective_to,
    pv.published_at
FROM compliance.policy_versions pv
WHERE pv.deleted_at IS NULL
  AND pv.policy_status = 'active';

CREATE OR REPLACE VIEW compliance.v_open_incidents AS
SELECT
    i.incident_id,
    i.tenant_id,
    i.incident_code,
    i.incident_type,
    i.incident_status,
    i.severity,
    i.title,
    i.state_code,
    i.detected_at,
    i.reported_at,
    i.assigned_to_user_id,
    i.legal_hold_applied
FROM compliance.incidents i
WHERE i.deleted_at IS NULL
  AND i.incident_status IN ('open', 'investigating', 'contained', 'escalated');

-- =========================================================
-- COMMENTS
-- =========================================================

COMMENT ON SCHEMA compliance IS 'Compliance schema for rules, checks, policy versions, legal holds, retention, exports, inspections, incidents, and audit logs.';
COMMENT ON TABLE compliance.compliance_rules IS 'Compliance rule catalog used for validation and blocking.';
COMMENT ON TABLE compliance.policy_versions IS 'Versioned compliance policies and rule bundles.';
COMMENT ON TABLE compliance.compliance_checks IS 'Compliance evaluation runs.';
COMMENT ON TABLE compliance.compliance_check_results IS 'Detailed findings for compliance checks.';
COMMENT ON TABLE compliance.legal_holds IS 'Legal hold records for preservation and blocking.';
COMMENT ON TABLE compliance.compliance_audit_events IS 'Evidence-grade audit trail for compliance operations.';
COMMENT ON TABLE compliance.retention_policies IS 'Retention rules for records and artifacts.';
COMMENT ON TABLE compliance.retention_jobs IS 'Scheduled/executed retention jobs.';
COMMENT ON TABLE compliance.regulatory_exports IS 'Exports prepared for regulators or audits.';
COMMENT ON TABLE compliance.inspections IS 'Regulatory inspection tracking.';
COMMENT ON TABLE compliance.incidents IS 'Compliance incidents and violations.';
COMMENT ON TABLE compliance.incident_actions IS 'Corrective actions for incidents.';
COMMENT ON TABLE compliance.compliance_audit_logs IS 'Unified audit log for compliance operations.';

-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================
-- ==================================================================================================================

