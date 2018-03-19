//MIT License

//Copyright(c) .NET Foundation and Contributors

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//
// https://github.com/dotnet-architecture/eShopOnContainers
//
// Modifications copyright(C) 2018 ei8/Elmer Bool

using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Animations.Base;
using Xamarin.Forms;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Helpers
{
    public static class EasingHelper
    {
        public static Easing GetEasing(EasingType type)
        {
            switch (type)
            {
                case EasingType.BounceIn:
                    return Easing.BounceIn;
                case EasingType.BounceOut:
                    return Easing.BounceOut;
                case EasingType.CubicIn:
                    return Easing.CubicIn;
                case EasingType.CubicInOut:
                    return Easing.CubicInOut;
                case EasingType.CubicOut:
                    return Easing.CubicOut;
                case EasingType.Linear:
                    return Easing.Linear;
                case EasingType.SinIn:
                    return Easing.SinIn;
                case EasingType.SinInOut:
                    return Easing.SinInOut;
                case EasingType.SinOut:
                    return Easing.SinOut;
                case EasingType.SpringIn:
                    return Easing.SpringIn;
                case EasingType.SpringOut:
                    return Easing.SpringOut;
            }

            return null;
        }
    }
}
