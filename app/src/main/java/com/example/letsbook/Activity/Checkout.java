package com.example.letsbook.Activity;

import androidx.appcompat.app.AppCompatActivity;
import androidx.cardview.widget.CardView;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.example.letsbook.ApiRoutes.ReservationApi;
import com.example.letsbook.ApiRoutes.TrainApi;
import com.example.letsbook.Modal.CheckRes;
import com.example.letsbook.Modal.UserRecord;
import com.example.letsbook.ModalDao.Record;
import com.example.letsbook.ModalDao.ReservationItem;
import com.example.letsbook.ModalDao.ReservationReq;
import com.example.letsbook.ModalDao.TrainItem;
import com.example.letsbook.ModalDao.TrainRes;
import com.example.letsbook.R;
import com.example.letsbook.RetroftService.RetrofitService;

import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class Checkout extends AppCompatActivity {
    private TextView tvSeatBookedDetail;
    private TextView tvTypeTrainCheck;
    private TextView tvNameTrain;
    private TextView tvTotal;
    private TextView tvSubTotal;
    private CardView cvProceedToPayAct;
    private ImageView btnBackReservation;
    private TrainItem trainItem;
    private CheckRes checkRes;
    private String  token;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_checkout);

        cvProceedToPayAct = findViewById(R.id.cvProceedToPayAct);
        tvSubTotal = findViewById(R.id.tvSubTotal);
        tvTotal = findViewById(R.id.tvTotal);
        tvNameTrain = findViewById(R.id.tvNameTrain);
        tvSeatBookedDetail = findViewById(R.id.tvSeatBookedDetail);
        tvTypeTrainCheck = findViewById(R.id.tvTypeTrainCheck);
        btnBackReservation = findViewById(R.id.btnBackReservation);

        // In your Checkout activity's onCreate method or wherever you want to access the values
        Bundle bundle = getIntent().getExtras();

        if (bundle != null) {
             trainItem = (TrainItem) bundle.getSerializable("trainItem");
            token = bundle.getString("token");
             checkRes = (CheckRes) bundle.getSerializable("checkRes");

            tvNameTrain.setText(trainItem.getTrainName());
            tvSeatBookedDetail.setText(checkRes.getSelectedSeat());
            tvTypeTrainCheck.setText(trainItem.getTrainType());

            tvTotal.setText("Rs 20000");
            tvSubTotal.setText("Rs 20000");
        }
        cvProceedToPayAct.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                reserveTrain();
            }
        });
    }
    private void reserveTrain(){
        RetrofitService retrofitService = new RetrofitService();
        ReservationApi makeReservation = retrofitService.getRetrofit().create(ReservationApi.class);
        Call<ReservationReq> call = makeReservation.createReservation(token, new ReservationReq(trainItem.getId(),checkRes.getSheduleId(),"moshdev2213@gmail.com",checkRes.getSelectedDate(),Integer.parseInt(checkRes.getSelectedSeat()),0));

        System.out.println(call.request()+ " "+ token+" "+trainItem.getId()+" "+checkRes.getSelectedDate()+" "+checkRes.getSheduleId()+" "+Integer.parseInt(checkRes.getSelectedSeat()));

        call.enqueue(new Callback<ReservationReq>() {
            @Override
            public void onResponse(Call<ReservationReq> call, Response<ReservationReq> response) {
                System.out.println(response.body());
                if (response.isSuccessful()) {
                    if (response.body() != null) {
                        ReservationReq reservationReq = response.body();
                        UserRecord userRecord = new UserRecord(token,new Record(reservationReq.getEmail()));
                        Intent intent = new Intent(Checkout.this, ThankYou.class);
                        Bundle bundle = new Bundle();
                        bundle.putSerializable("userRecord", userRecord);
                        intent.putExtras(bundle);
                        startActivity(intent);
                        finish();
                    }
                } else {
                    Toast.makeText(getApplicationContext(), "Invalid Credentials", Toast.LENGTH_SHORT).show();
//                    progressLoader.dismissProgressLoader();
                }
            }
            @Override
            public void onFailure(Call<ReservationReq> call, Throwable t) {
                Toast.makeText(getApplicationContext(), "Invalid Down", Toast.LENGTH_SHORT).show();
            }
        });
    }

}