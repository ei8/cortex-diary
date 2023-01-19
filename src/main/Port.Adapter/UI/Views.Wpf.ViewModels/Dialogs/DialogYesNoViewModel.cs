/*
    This file is part of the d# project.
    Copyright (c) 2016-2018 ei8
    Authors: ei8
     This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License version 3
    as published by the Free Software Foundation with the addition of the
    following permission added to Section 15 as permitted in Section 7(a):
    FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
    EI8. EI8 DISCLAIMS THE WARRANTY OF NON INFRINGEMENT OF THIRD PARTY RIGHTS
     This program is distributed in the hope that it will be useful, but
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
    or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License
    along with this program; if not, see http://www.gnu.org/licenses or write to
    the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
    Boston, MA, 02110-1301 USA, or download the license from the following URL:
    https://github.com/ei8/cortex-diary/blob/master/LICENSE
     The interactive user interfaces in modified source and object code versions
    of this program must display Appropriate Legal Notices, as required under
    Section 5 of the GNU Affero General Public License.
     You can be released from the requirements of the license by purchasing
    a commercial license. Buying such a license is mandatory as soon as you
    develop commercial activities involving the d# software without
    disclosing the source code of your own applications.
     For more information, please contact ei8 at this address: 
     support@ei8.works
 */

using ReactiveUI;
using System.Reactive;
using System.Windows.Input;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs
{
    /// <summary>
    /// https://www.c-sharpcorner.com/article/dialogs-in-wpf-mvvm/
    /// </summary>
    public class DialogYesNoViewModel : DialogViewModelBase
    {
        public ReactiveCommand<Unit, Unit> YesCommand { get; }

        public ReactiveCommand<Unit, Unit> NoCommand { get; }

        public DialogYesNoViewModel(string message)
            : base(message)
        {
            this.UserDialogResult = Dialogs.DialogResult.Undefined;
            this.YesCommand = ReactiveCommand.Create(OnYesClicked);
            this.NoCommand = ReactiveCommand.Create(OnNoClicked);
        }

        private void OnYesClicked()
        {
            this.UserDialogResult = Dialogs.DialogResult.Yes;
            this.DialogResult = true;
        }

        private void OnNoClicked()
        {
            this.UserDialogResult = Dialogs.DialogResult.No;
            this.DialogResult = false;
        }
    }
}
