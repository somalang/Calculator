//using System;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;
//using System.Windows;
//using System.Windows.Input;

//namespace Calculator.UI.ViewModels
//{
//    public class BaseConverterViewModel : INotifyPropertyChanged
//    {
//        private string inputValue;
//        private int currentBase = 10;

//        public string InputValue
//        {
//            get => inputValue ?? "0";
//            set
//            {
//                inputValue = value;
//                OnPropertyChanged();
//                UpdateOutputs();
//            }
//        }

//        // Input base properties
//        public bool IsBinaryInput
//        {
//            get => currentBase == 2;
//            set
//            {
//                if (value)
//                {
//                    currentBase = 2;
//                    OnPropertyChanged();
//                    OnPropertyChanged(nameof(IsOctalInput));
//                    OnPropertyChanged(nameof(IsDecimalInput));
//                    OnPropertyChanged(nameof(IsHexInput));
//                    UpdateDigitAvailability();
//                    UpdateOutputs();
//                }
//            }
//        }

//        public bool IsOctalInput
//        {
//            get => currentBase == 8;
//            set
//            {
//                if (value)
//                {
//                    currentBase = 8;
//                    OnPropertyChanged();
//                    OnPropertyChanged(nameof(IsBinaryInput));
//                    OnPropertyChanged(nameof(IsDecimalInput));
//                    OnPropertyChanged(nameof(IsHexInput));
//                    UpdateDigitAvailability();
//                    UpdateOutputs();
//                }
//            }
//        }

//        public bool IsDecimalInput
//        {
//            get => currentBase == 10;
//            set
//            {
//                if (value)
//                {
//                    currentBase = 10;
//                    OnPropertyChanged();
//                    OnPropertyChanged(nameof(IsBinaryInput));
//                    OnPropertyChanged(nameof(IsOctalInput));
//                    OnPropertyChanged(nameof(IsHexInput));
//                    UpdateDigitAvailability();
//                    UpdateOutputs();
//                }
//            }
//        }

//        public bool IsHexInput
//        {
//            get => currentBase == 16;
//            set
//            {
//                if (value)
//                {
//                    currentBase = 16;
//                    OnPropertyChanged();
//                    OnPropertyChanged(nameof(IsBinaryInput));
//                    OnPropertyChanged(nameof(IsOctalInput));
//                    OnPropertyChanged(nameof(IsDecimalInput));
//                    UpdateDigitAvailability();
//                    UpdateOutputs();
//                }
//            }
//        }

//        // Digit availability properties
//        private bool isBinaryDigitEnabled = true;
//        private bool isOctalDigitEnabled = true;
//        private bool isDecimalDigitEnabled = true;
//        private bool isHexDigitEnabled = true;

//        public bool IsBinaryDigitEnabled
//        {
//            get => isBinaryDigitEnabled;
//            set
//            {
//                isBinaryDigitEnabled = value;
//                OnPropertyChanged();
//            }
//        }

//        public bool IsOctalDigitEnabled
//        {
//            get => isOctalDigitEnabled;
//            set
//            {
//                isOctalDigitEnabled = value;
//                OnPropertyChanged();
//            }
//        }

//        public bool IsDecimalDigitEnabled
//        {
//            get => isDecimalDigitEnabled;
//            set
//            {
//                isDecimalDigitEnabled = value;
//                OnPropertyChanged();
//            }
//        }

//        public bool IsHexDigitEnabled
//        {
//            get => isHexDigitEnabled;
//            set
//            {
//                isHexDigitEnabled = value;
//                OnPropertyChanged();
//            }
//        }

//        // Output properties
//        //public

//    }