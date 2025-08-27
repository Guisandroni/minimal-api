


namespace minimal_api.Domain.ModelsViews
{
    public struct ExceptionValidation
    {
        public List<string> Mensagens { get; set; }

        public ExceptionValidation()
        {
            Mensagens = new List<string>();
        }
    }
}