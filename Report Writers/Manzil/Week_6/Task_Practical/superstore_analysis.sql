-- Superstore Sales Analysis
-- Week 1 - Report Analyst
-- SQL Practice


-- Query 1: Total Sales by Region
SELECT
    Region,
    ROUND(SUM(CAST(Sales AS REAL)), 2) AS Total_Sales
FROM orders
GROUP BY Region
ORDER BY Total_Sales DESC;


-- Query 2: Total Profit by Category
SELECT
    Category,
    ROUND(SUM(CAST(Profit AS REAL)), 2) AS Total_Profit
FROM orders
GROUP BY Category
ORDER BY Total_Profit DESC;


-- Query 3: Sales Trend by Year
SELECT
    SUBSTR("Order Date", -4) AS Year,
    ROUND(SUM(CAST(Sales AS REAL)), 2) AS Total_Sales
FROM orders
GROUP BY Year
ORDER BY Year;


-- Query 4: Top 10 Products by Sales
SELECT
    "Product Name",
    ROUND(SUM(CAST(Sales AS REAL)), 2) AS Total_Sales
FROM orders
GROUP BY "Product Name"
ORDER BY Total_Sales DESC
LIMIT 10;


-- Query 5: Profit by Region
SELECT
    Region,
    ROUND(SUM(CAST(Profit AS REAL)), 2) AS Total_Profit
FROM orders
GROUP BY Region
ORDER BY Total_Profit DESC;


-- Query 6: Sales and Profit by Customer Segment
SELECT
    Segment,
    ROUND(SUM(CAST(Sales AS REAL)), 2) AS Total_Sales,
    ROUND(SUM(CAST(Profit AS REAL)), 2) AS Total_Profit
FROM orders
GROUP BY Segment
ORDER BY Total_Sales DESC;


-- Query 7: Top 10 Loss-Making Transactions
SELECT
    "Product Name",
    ROUND(CAST(Sales AS REAL), 2) AS Sales,
    ROUND(CAST(Profit AS REAL), 2) AS Profit
FROM orders
WHERE CAST(Profit AS REAL) < 0
ORDER BY Profit ASC
LIMIT 10;


-- Query 8: Average Sales and Profit
SELECT
    ROUND(AVG(CAST(Sales AS REAL)), 2) AS Average_Sales,
    ROUND(AVG(CAST(Profit AS REAL)), 2) AS Average_Profit
FROM orders;