using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class KalmanFilterSimple1D
{
    public double X0 { get; private set; } // predicted state
    public double P0 { get; private set; } // predicted covariance

    public double F { get; private set; } // factor of real value to previous real value
    public double Q { get; private set; } // measurement noise
    public double H { get; private set; } // factor of measured value to real value
    public double R { get; private set; } // environment noise

    public double State { get; private set; }
    public double Covariance { get; private set; }

    public KalmanFilterSimple1D(double q, double r, double f = 1, double h = 1)
    {
        Q = q;
        R = r;
        F = f;
        H = h;
    }

    public void SetState(double state, double covariance)
    {
        State = state;
        Covariance = covariance;
    }

    public void Correct(double data)
    {
        //time update - prediction
        X0 = F * State;
        P0 = F * Covariance * F + Q;

        //measurement update - correction
        var K = H * P0 / (H * P0 * H + R);
        State = X0 + K * (data - H * X0);
        Covariance = (1 - K * H) * P0;
    }
}



// Применение...

//var fuelData = GetData();
//var filtered = new List<double>();

//var kalman = new KalmanFilterSimple1D(f: 1, h: 1, q: 2, r: 15); // задаем F, H, Q и R
//kalman.SetState(fuelData[0], 0.1); // Задаем начальные значение State и Covariance
//foreach(var d in fuelData)
//{
//    kalman.Correct(d); // Применяем алгоритм

//    filtered.Add(kalman.State); // Сохраняем текущее состояние 
//}
