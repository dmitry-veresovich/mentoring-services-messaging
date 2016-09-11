namespace Services.Services
{
    interface IPdfService
    {
        void NewFile();
        void AddPage(string path);
        void SaveFile();
    }
}
