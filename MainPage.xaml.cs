using SharpHook.Native;
using SharpHook.Reactive;
using SharpHook;
using System.Reactive.Linq;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
		IReactiveGlobalHook _keyboardHook;
        public MainPage()
        {
            InitializeComponent();
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (_keyboardHook is null)
			{
				_keyboardHook = new SimpleReactiveGlobalHook(GlobalHookType.Keyboard, runAsyncOnBackgroundThread: true);

				// Subscribe to Ctrl + N
				_keyboardHook.KeyPressed
					.Where(e => e.Data.KeyCode == KeyCode.VcN && e.RawEvent.Mask.HasCtrl())
					.Subscribe(OnCtrlNPressed);

				// Subscribe to all key presses
				_keyboardHook.KeyPressed.Subscribe(e =>
				{
					MainThread.BeginInvokeOnMainThread(() =>
					{
						StatusEntry.Text = $"PRESSED - Key: {e.Data.KeyCode}, Mask: {e.RawEvent.Mask}";
					});
				});

				await _keyboardHook.RunAsync();
			}
		}

		private void OnCtrlNPressed(KeyboardHookEventArgs args)
		{
			var newWindow = new Window()
			{
				Page = new MainPage()
			};

			MainThread.BeginInvokeOnMainThread(() =>
			{
				Application.Current?.OpenWindow(newWindow);
			});
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			_keyboardHook?.Dispose();
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			//disable hook
			_keyboardHook?.Dispose();
		}
	}

}
