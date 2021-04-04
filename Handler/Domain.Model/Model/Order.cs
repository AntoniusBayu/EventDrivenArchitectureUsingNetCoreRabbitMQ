namespace Domain.Model
{
    public class Order : BaseModel
    {
        public int OrderID { get; set; }
        public int Quantity { get; set; }
        public int ProductID { get; set; }
    }
}
