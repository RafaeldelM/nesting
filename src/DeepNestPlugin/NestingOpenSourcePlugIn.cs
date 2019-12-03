using System;
using Rhino.PlugIns;
using RhinoWindows.Controls;
using DockBarPanel = NestingOpenSource.Panel.DockBarPanel;

namespace NestingOpenSource
{
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    public class NestingOpenSourcePlugIn : Rhino.PlugIns.PlugIn
    {

        private WpfDockBar m_wpf_bar;


        public NestingOpenSourcePlugIn()
        {
            Instance = this;
        }

        ///<summary>Gets the only instance of the NestingOpenSourcePlugIn plug-in.</summary>
        public static NestingOpenSourcePlugIn Instance
        {
            get; private set;
        }

        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {

            CreateMyDockBar();

            return base.OnLoad(ref errorMessage);
        }

        private void CreateMyDockBar()
        {
            var create_options = new DockBarCreateOptions
            {
                DockLocation = DockBarDockLocation.Right,
                Visible = false,
                DockStyle = DockBarDockStyle.Any,
                FloatPoint = new System.Drawing.Point(100, 100)
            };


            m_wpf_bar = new WpfDockBar();
            m_wpf_bar.Create(create_options);
        }


        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and maintain plug-in wide options in a document.
    }


    /// <summary>
    /// WpfDockBar dockbar class
    /// </summary>
    internal class WpfDockBar : DockBar
    {
        public static Guid BarId => new Guid("{5faf6cd0-c387-46cd-83b3-1445c126cf55}");
        public WpfDockBar() : base(NestingOpenSourcePlugIn.Instance, BarId, "DeepNest")
        {
            SetContentControl(new WpfHost(new DockBarPanel(), null));
        }
    }
}