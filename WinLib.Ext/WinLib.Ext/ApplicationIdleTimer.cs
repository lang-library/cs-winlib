using System;
using System.Windows.Forms;
namespace NAMESPACE;
public class ApplicationIdleTimer
{
    public static MessageFilter Filter;
    public event EventHandler Tick;
    public ApplicationIdleTimer(int interval)
    {
        Filter = new MessageFilter(this, interval);
        Application.AddMessageFilter(Filter);
        Application.Idle += new EventHandler(Application_Idle);
    }
    private void Application_Idle(Object sender, EventArgs e)
    {
        Filter.IdleTimer.Start();
    }
    private void Timer_Tick(object sender, EventArgs e)
    {
        Filter.IdleTimer.Stop();
        EventHandler handler = Tick;
        handler?.Invoke(this, e);
    }
    public class MessageFilter : IMessageFilter
    {
        public ApplicationIdleTimer owner;
        public System.Windows.Forms.Timer IdleTimer = new System.Windows.Forms.Timer();
        enum WindowMessage
        {
            WM_TIMER = 0x0113,
        }
        public MessageFilter(ApplicationIdleTimer owner, int interval)
        {
            this.owner = owner;
            IdleTimer.Interval = interval;
            IdleTimer.Tick += owner.Timer_Tick;
        }
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg != (int)WindowMessage.WM_TIMER) IdleTimer.Stop();
            return false;
        }
    }
}
