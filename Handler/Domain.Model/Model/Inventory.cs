namespace Domain.Model
{
    public class Inventory : BaseModel
    {
        public int ProductID { get; set; }
        public int Stock { get; set; }
        public int OrderID { get; set; }
    }
}
