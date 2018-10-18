// DEL: using Spiker.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.WinForms
{
    public class Helper
    {
        public static string GetNewShortGuid()
        {
            return Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "").Substring(0, 5);
        }

        // TODO: public static Neuron GetNeuronByData(string value, NeuronCollection neurons)
        //{
        //    return neurons.First(n => n.Data == value);
        //}

        //internal static bool IsSelectionNeuron(ISelectionService selectionService)
        //{
        //    return (selectionService.SelectedObjects.FirstOrDefault() as Neuron) != null;
        //}
    }
}
