﻿#pragma checksum "..\..\TGS.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "279975FFC7E68DA011A00C7A34E0CEB6953A8BF1"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using SC03;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SC03 {
    
    
    /// <summary>
    /// TGS
    /// </summary>
    public partial class TGS : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\TGS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TB_key;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\TGS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TB_send_1;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\TGS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TB_recv_1;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\TGS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TB_send_2;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\TGS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TB_recv_2;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\TGS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TB_log;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SC03;component/tgs.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\TGS.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.TB_key = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.TB_send_1 = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.TB_recv_1 = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.TB_send_2 = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.TB_recv_2 = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.TB_log = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            
            #line 25 "..\..\TGS.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Init);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

