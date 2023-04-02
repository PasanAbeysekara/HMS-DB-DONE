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

				var c1 = context.Appointments.Where(x => x.PatientId == tmp.Id).ToList();
				if (c1 != null) c1.ForEach(y => { appointments.Add(y); });
				else MessageBox.Show("This patient have no Appointments");
				var c2 = context.Prescriptions.Where(x => x.PatientId == tmp.Id).ToList();
				if (c2 != null) c2.ForEach(p => { prescriptions.Add(p); });
				else MessageBox.Show("This patient have no Prescriptions");
			}
		}
	}
}
