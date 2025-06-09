CREATE DATABASE OnlinePaymentSystem;
GO

USE OnlinePaymentSystem;

CREATE TABLE users (
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    username NVARCHAR(50) NOT NULL UNIQUE,
    full_name NVARCHAR(255) NOT NULL,
    password NVARCHAR(64) NOT NULL -- SHA-256 hash
);

CREATE TABLE accounts (
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    account_number CHAR(22) NOT NULL UNIQUE,
    balance DECIMAL(18,2) NOT NULL DEFAULT 0.00
);

CREATE TABLE user_accounts (
    user_id INT NOT NULL,
    account_id INT NOT NULL,
    PRIMARY KEY (user_id, account_id),
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (account_id) REFERENCES accounts(id)
);

CREATE TABLE payments (
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    from_account_id INT NOT NULL,
    to_account_id INT NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    description NVARCHAR(32) NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'PENDING',
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (from_account_id) REFERENCES accounts(id),
    FOREIGN KEY (to_account_id) REFERENCES accounts(id),
    CONSTRAINT CHK_PaymentStatus CHECK (status IN ('PENDING', 'PROCESSED', 'CANCELLED')),
    CONSTRAINT CHK_PaymentAmount CHECK (amount > 0)
);

CREATE INDEX IX_user_accounts_user_id ON user_accounts(user_id);
CREATE INDEX IX_user_accounts_account_id ON user_accounts(account_id);
CREATE INDEX IX_payments_user_id ON payments(user_id);
CREATE INDEX IX_payments_from_account_id ON payments(from_account_id);
CREATE INDEX IX_payments_to_account_id ON payments(to_account_id);
CREATE INDEX IX_payments_status ON payments(status);
CREATE INDEX IX_payments_created_at ON payments(created_at);