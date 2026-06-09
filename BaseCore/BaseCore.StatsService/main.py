"""
BaseCore Stats Service - Python FastAPI (port 5003)
Aggregates dashboard statistics from the SQL Server database.
Run: uvicorn main:app --host 0.0.0.0 --port 5003
"""

from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
import pyodbc

app = FastAPI(title="BaseCore Stats Service", version="1.0.0")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["GET", "OPTIONS"],
    allow_headers=["*"],
)

# Windows Integrated Authentication (same as the C# services)
CONN_STR = (
    "DRIVER={ODBC Driver 17 for SQL Server};"
    "SERVER=DESKTOP-L6EUOCD\\SQLEXPRESS;"
    "DATABASE=BaseCoreSales;"
    "Trusted_Connection=yes;"
    "TrustServerCertificate=yes;"
)


def get_count(cursor, table: str, where: str = "") -> int:
    sql = f"SELECT COUNT(*) FROM [{table}]"
    if where:
        sql += f" WHERE {where}"
    cursor.execute(sql)
    return cursor.fetchone()[0]


@app.get("/api/stats")
def get_stats():
    try:
        conn = pyodbc.connect(CONN_STR, timeout=5)
        cursor = conn.cursor()

        stats = {
            "products":    get_count(cursor, "Products", "IsActive = 1"),
            "variants":    get_count(cursor, "ProductVariants"),
            "categories":  get_count(cursor, "Categories", "IsActive = 1"),
            "brands":      get_count(cursor, "Brands"),
            "collections": get_count(cursor, "Collections", "IsActive = 1"),
            "orders":      get_count(cursor, "Orders"),
            "promotions":  get_count(cursor, "Promotions", "IsActive = 1"),
            "reviews":     get_count(cursor, "Reviews", "IsApproved = 1"),
            "users":       get_count(cursor, "Users", "IsActive = 1"),
        }

        # Doanh số theo trạng thái đơn hàng (OrderStatus enum int)
        cursor.execute(
            "SELECT Status, COUNT(*) FROM [Orders] GROUP BY Status"
        )
        stats["ordersByStatus"] = {str(row[0]): row[1] for row in cursor.fetchall()}

        # Cảnh báo low stock — variant có (Stock - ReservedStock) ≤ 5
        cursor.execute(
            "SELECT COUNT(*) FROM [ProductVariants] WHERE (Stock - ReservedStock) <= 5"
        )
        stats["lowStockVariants"] = cursor.fetchone()[0]

        cursor.close()
        conn.close()
        return stats

    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Database error: {str(e)}")


@app.get("/health")
def health():
    return {"status": "ok", "service": "stats"}


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=5003)
