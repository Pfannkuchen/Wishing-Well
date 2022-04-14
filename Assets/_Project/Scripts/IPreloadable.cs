public interface IPreloadable
{
    LoadingIcon Loader { get; }
    bool PreloadFinished { get; }
    void HideImmediately();
    void Show();
}