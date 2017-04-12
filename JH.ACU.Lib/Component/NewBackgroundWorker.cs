using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace JH.ACU.Lib.Component
{
    public partial class NewBackgroundWorker : BackgroundWorker
    {
        public NewBackgroundWorker()
        {
            InitializeComponent();
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        public NewBackgroundWorker(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;

        }
    }
}
