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

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using works.ei8.Cortex.Diary.Application.Identity;
using works.ei8.Cortex.Diary.Application.OpenUrl;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Dialogs
{
    /// <summary>
    /// https://www.c-sharpcorner.com/article/dialogs-in-wpf-mvvm/
    /// </summary>
    public class DialogService : IDialogService
    {
        public Task<bool?> ShowDialogSelectNeurons(string message, string avatarUrl, object owner, bool allowMultiSelect, out IEnumerable<Neuron> result)
        {
            return this.ShowDialog<IEnumerable<Neuron>>(new DialogSelectNeuronsViewModel(message, avatarUrl, allowMultiSelect), owner, out result, "Select Neuron(s)", 500, 600);
        }

        public Task<bool?> ShowDialogYesNo(string message, object owner, out DialogResult result)
        {
            return this.ShowDialog<DialogResult>(new DialogYesNoViewModel(message), owner, out result);
        }

        public Task<bool?> ShowDialogTextInput(string message, object owner, out string result)
        {
            return this.ShowDialog<string>(new DialogTextInputViewModel(message), owner, out result);
        }
        
        public Task<bool?> ShowLogin(ISettingsService settingsService, IOpenUrlService openUrlService, IIdentityService identityService, object owner, out bool result)
        {
            return this.ShowDialog<bool>(new LoginViewModel(settingsService, openUrlService, identityService), owner, out result, "Avatar Server Log In", 800, 630);
        }

        private Task<bool?> ShowDialog<T>(DialogViewModelBase vm, object owner, out T result, string title = null, int maxWidth = 0, int maxHeight = 0)
        {
            DialogWindow win = new DialogWindow();
            if (owner != null)
                win.Owner = owner as Window;
            if (maxHeight > 0)
                win.MaxHeight = maxHeight;
            if (maxWidth > 0)
                win.MaxWidth = maxWidth;
            if (!string.IsNullOrEmpty(title))
                win.Title = title;
            win.DataContext = vm;            
            win.ShowDialog();
            result = (T) vm.UserDialogResult;
            return Task.FromResult(vm.DialogResult);
        }
    }
}
