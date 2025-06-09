USE OnlinePaymentSystem;
GO

INSERT INTO users (username, full_name, password) VALUES
('ivan.petrov', 'Ivan Petrov', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92'), -- password: 123456
('maria.georgieva', 'Maria Georgieva', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92'), -- password: 123456
('georgi.dimitrov', 'Georgi Dimitrov', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92'); -- password: 123456

INSERT INTO accounts (account_number, balance) VALUES
('BG12345678901234567890', 5000.00),
('BG23456789012345678901', 3000.00),
('BG34567890123456789012', 7500.00),
('BG45678901234567890123', 2500.00),
('BG56789012345678901234', 10000.00);

INSERT INTO user_accounts (user_id, account_id) VALUES
(1, 1), -- Ivan has account 1
(1, 2), -- Ivan has account 2
(2, 2), -- Maria has account 2 (shared with Ivan)
(2, 3), -- Maria has account 3
(3, 4), -- Georgi has account 4
(3, 5); -- Georgi has account 5