namespace ModelLibrary.Shared.Func
{
    public struct PizzaWithMakeTime
    {
        /*In the current model the PizzaId and size will not be used in the logic, so could be removed.
         * I left it in, because thisway, it's more easy to read the code's meaning.
         * + it would make sense to keep them around for full functionality*/
        public int PizzaId { get; set; }
        public PizzaSize Size { get; set; }
        public double TimeToMake { get; set; }
    }
}
