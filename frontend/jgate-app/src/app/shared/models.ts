export interface Product { id: string; name: string; sku?: string; description?: string; categoryId: string; categoryName: string; supplierId?: string; supplierName?: string; unitPrice: number; unit: string; minStockLevel: number; isActive: boolean; requiresColdStorage: boolean; }
export interface Category { id: string; name: string; description?: string; }
export interface Supplier { id: string; name: string; contactPerson?: string; email?: string; phone?: string; address?: string; isActive: boolean; }
export interface StockLevel { id: string; productId: string; productName: string; warehouseId: string; warehouseName: string; quantityOnHand: number; quantityReserved: number; quantityAvailable: number; }
export interface StockMovement { id: string; productId: string; productName: string; movementType: string; quantity: number; unitCost?: number; referenceNumber?: string; notes?: string; movementDate: string; }
export interface Order { id: string; orderNumber: string; customerId: string; customerName: string; status: string; orderDate: string; requiredDate?: string; shippingAddress?: string; notes?: string; subTotal: number; taxAmount: number; totalAmount: number; items: OrderItem[]; }
export interface OrderItem { id: string; productId: string; productName: string; quantity: number; unit: string; unitPrice: number; discount: number; lineTotal: number; }
export interface Customer { id: string; name: string; contactPerson?: string; email?: string; phone?: string; address?: string; taxId?: string; isActive: boolean; }
export interface Driver { id: string; firstName: string; lastName: string; email?: string; phone?: string; licenseNumber?: string; isAvailable: boolean; isActive: boolean; }
export interface Vehicle { id: string; plateNumber: string; make: string; model: string; year: number; type: string; maxLoadKg?: number; hasTemperatureControl: boolean; isAvailable: boolean; isActive: boolean; }
export interface DeliveryRoute { id: string; routeCode: string; name: string; plannedDate: string; driverId?: string; driverName?: string; vehicleId?: string; vehiclePlate?: string; notes?: string; }
export interface DeliveryOrder { id: string; stopId?: string; orderId: string; orderNumber: string; status: string; deliveredAt?: string; proofOfDelivery?: string; failureReason?: string; }
export interface WorkTask { id: string; title: string; description?: string; categoryId: string; categoryName: string; status: string; priority: string; dueDate?: string; assignedToUserId?: string; assignedToUserName?: string; notes?: string; comments: TaskComment[]; }
export interface TaskComment { id: string; userId: string; userName: string; content: string; postedAt: string; }
export interface TaskCategory { id: string; name: string; description?: string; color?: string; }
export interface DashboardStats { totalProducts: number; lowStockCount: number; pendingOrders: number; activeDeliveries: number; openTasks: number; }
