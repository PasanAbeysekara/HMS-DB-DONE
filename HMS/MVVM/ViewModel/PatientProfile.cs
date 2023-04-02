using CommunityToolkit.Mvvm.ComponentModel;
using HMS.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HMS.MVVM.ViewModel
{
	public partial class PatientProfileVM : ObservableObject
	{

		[ObservableProperty]
		public string name;

		[ObservableProperty]
		public string age;

		[ObservableProperty]
		public string gender;

		[ObservableProperty]
		public string phone;

		[ObservableProperty]
		public string address;

		[ObservableProperty]
		public string weight;

		[ObservableProperty]
		public string height;

		[ObservableProperty]
		public string blood;

		// Billing
		[ObservableProperty]
		public string doctorFee;

		[ObservableProperty]
		public string testFee;

		[ObservableProperty]
		public string hospitalFee;

		[ObservableProperty]
		public string totalFee;

		//[ObservableProperty]
		//public List<Appointment> appointments;

		private ObservableCollection<Appointment> appointments = new ObservableCollection<Appointment>();


		public ObservableCollection<Appointment> Appointments
		{
			get => appointments;
			set
			{
				if (appointments != value)
				{
					appointments = value;
					OnPropertyChanged();
				}
			}
		}

		//[ObservableProperty]
		//public ObservableCollection<Prescription> prescriptions;


		private ObservableCollection<Prescription> prescriptions = new ObservableCollection<Prescription>();


		public ObservableCollection<Prescription> Prescriptions
		{
			get => prescriptions;
			set
			{
				if (prescriptions != value)
				{
					prescriptions = value;
					OnPropertyChanged();
				}
			}
		}

		public PatientProfileVM()
		{
			using (DataContext context = new DataContext())
			{
				Patient tmp = context.Patients.Single(x => x.IsPatientSelected == true);
				tmp.IsPatientSelected = false;
				context.SaveChanges();

				// name
				name = tmp.FullName;

				// age
				string birthDateString = tmp.BirthDay;
				DateTime birthDate = DateTime.ParseExact(birthDateString, "MM/dd/yyyy", null);
				DateTime today = DateTime.Today;
				int _age = today.Year - birthDate.Year;
				if (birthDate > today.AddYears(-_age))
				{
					_age--;
				}
				age = $"{_age} years";

				//gender
				gender = tmp.Gender == 'M' ? "Male" : "Female";

				//phone
				phone = tmp.Phone;

				//address
				address = tmp.Address;

				//weight
				weight = $"{tmp.Weight} kg";

				//height
				height = $"{tmp.Height} cm";

				//blood
				blood = tmp.BloodGroup;

				var apps = context.Appointments.Where(x => x.PatientId == tmp.Id).ToList();
				if (apps != null) apps.ForEach(y => { appointments.Add(y); });
				else MessageBox.Show("This patient have no Appointments");
				var prescs = context.Prescriptions.Where(x => x.PatientId == tmp.Id).ToList();
				if (prescs != null) prescs.ForEach(p => { prescriptions.Add(p); });
				else MessageBox.Show("This patient have no Prescriptions");

				//Billing
				double _docFee=0;
				foreach(var app in apps)
				{
					_docFee += context.Doctors.Single(x => x.Id == app.DoctorId).Fee;
				}
				doctorFee = $"Doctor Fee             : LKR {_docFee}";
				
				double _testFee=0;
				foreach (var presc in prescs)
				{
					//MessageBox.Show(presc.PrescribedDate.ToString());
					foreach (var medTest in context.MedicalTests.Where(x => x.PrescriptionId == presc.Id))
					{
						//MessageBox.Show("medTest.Description");
						_testFee += context.Tests.Single(x => x.Id == medTest.TestId).Fee;
					}

					//foreach (var medTest in context.MedicalTests)
					//{
					//	MessageBox.Show($"Med test pres id : {medTest.PrescriptionId}\nPresc Id : {presc.Id}");
					//}

				}
				testFee = $"Test Fee                  : LKR {_testFee}";

				hospitalFee = $"Hospital Fee (10%) : LKR {(_docFee+_testFee)*0.1}";

				totalFee = $"Total Fee                 : LKR { _docFee + _testFee + (_docFee + _testFee) * 0.1}";

			}
		}
	}
}
