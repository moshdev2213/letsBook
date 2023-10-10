package com.example.letsbook.Activity;

import androidx.appcompat.app.AppCompatActivity;
import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;

import android.app.DatePickerDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.DatePicker;
import android.widget.ImageView;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import com.example.letsbook.ApiRoutes.ReservationApi;
import com.example.letsbook.ApiRoutes.SheduleApi;
import com.example.letsbook.CallbBacks.SheduleCallback;
import com.example.letsbook.Modal.CheckRes;
import com.example.letsbook.ModalDao.ReservationReq;
import com.example.letsbook.ModalDao.SheduleItem;
import com.example.letsbook.ModalDao.SheduleRes;
import com.example.letsbook.ModalDao.TrainItem;
import com.example.letsbook.ModalDao.TrainRes;
import com.example.letsbook.R;
import com.example.letsbook.RetroftService.RetrofitService;

import java.util.Calendar;
import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class TrainDetail extends AppCompatActivity {
    private TrainItem trainItem;
    private String token;
    private TextView tvTrainTypeDetail;
    private TextView tvTrainNameDetail;
    private TextView tvTrainFromDetail;
    private TextView tvSeatCal;
    private TextView tvTrainToDetail;
    private Spinner spinnerSeatSelect;
    private Spinner spinnerSheduSelect;

    private CardView cvBackBtn;
    private CardView cvBuyBtn;
    private ImageView imgBackBtn;
    private String sheduleId;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_train_detail);

        Bundle bundle = getIntent().getExtras();

        if (bundle != null) {
            trainItem = (TrainItem) bundle.getSerializable("train");
            token =  bundle.getString("token");
        }

        cvBackBtn = findViewById(R.id.cvBackBtn);
        cvBuyBtn = findViewById(R.id.cvBuyBtn);
        tvTrainTypeDetail = findViewById(R.id.tvTrainTypeDetail);
        tvTrainNameDetail = findViewById(R.id.tvTrainNameDetail);
        tvTrainFromDetail = findViewById(R.id.tvTrainFromDetail);
        tvSeatCal = findViewById(R.id.tvSeatCal);
        tvTrainToDetail = findViewById(R.id.tvTrainToDetail);
        spinnerSeatSelect = findViewById(R.id.spinnerSeatSelect);
        spinnerSheduSelect = findViewById(R.id.spinnerSheduSelect);
        imgBackBtn = findViewById(R.id.imgBackBtn);

        tvTrainTypeDetail.setText(trainItem.getTrainType());
        tvTrainNameDetail.setText(trainItem.getTrainName());
        tvTrainFromDetail.setText(trainItem.getFromStation());
        tvTrainToDetail.setText(trainItem.getToStation());

        imgBackBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                finish();
            }
        });
        cvBackBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                finish();
            }
        });

        cvBuyBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                // Get the selected item from spinnerSeatSelect
                String selectedSeat = spinnerSeatSelect.getSelectedItem().toString();

                // Get the selected item from spinnerSheduSelect
                String selectedSchedule = spinnerSheduSelect.getSelectedItem().toString();

                //get the shedules

                getSheduleId(selectedSchedule, new SheduleCallback() {
                    @Override
                    public void onLoadGetSheduleData(SheduleItem sheduleItem) {
                        sheduleId = sheduleItem.getId();
                    }
                });

                // Get the current date
                Calendar calendar = Calendar.getInstance();
                int year = calendar.get(Calendar.YEAR);
                int month = calendar.get(Calendar.MONTH);
                int day = calendar.get(Calendar.DAY_OF_MONTH);

                // Calculate the maximum date (30 days from the current date)
                calendar.add(Calendar.DAY_OF_MONTH, 30);
                int maxYear = calendar.get(Calendar.YEAR);
                int maxMonth = calendar.get(Calendar.MONTH);
                int maxDay = calendar.get(Calendar.DAY_OF_MONTH);

                DatePickerDialog datePickerDialog = new DatePickerDialog(
                        TrainDetail.this, R.style.DialogTheme,
                        new DatePickerDialog.OnDateSetListener() {
                            @Override
                            public void onDateSet(DatePicker view, int selectedYear, int selectedMonth, int selectedDay) {
                                // Handle the selected date
                                Calendar selectedDateCalendar = Calendar.getInstance();
                                selectedDateCalendar.set(selectedYear, selectedMonth, selectedDay);

                                // Check if the selected date is within the allowed range
                                Calendar minDateCalendar = Calendar.getInstance();
                                Calendar maxDateCalendar = Calendar.getInstance();
                                maxDateCalendar.add(Calendar.DAY_OF_MONTH, 30); // 30 days from the current date

                                if (selectedDateCalendar.after(minDateCalendar) && selectedDateCalendar.before(maxDateCalendar)) {
                                    // Handle the selected date
                                    String selectedDate = selectedYear + "." + (selectedMonth + 1) + "." + selectedDay;
                                    CheckRes checkRes = new CheckRes(selectedDate,selectedSeat,selectedSchedule,sheduleId);

                                    Intent intent = new Intent(TrainDetail.this, Checkout.class);
                                    Bundle bundle = new Bundle();
                                    bundle.putSerializable("trainItem", trainItem);
                                    bundle.putSerializable("token", token);
                                    bundle.putSerializable("checkRes", checkRes);
                                    intent.putExtras(bundle);
                                    startActivity(intent);
                                    finish();
                                }else{
                                    // The selected date is not within the allowed range
                                    // Show an error message or perform appropriate actions
                                    Toast.makeText(TrainDetail.this, "Invalid selection. Allowed Only For A Month", Toast.LENGTH_SHORT).show();
                                }
                            }
                        }, year, month, day);

                // Show the DatePickerDialog
                datePickerDialog.show();

                // Prevent the dialog from closing when touched outside
                datePickerDialog.setCanceledOnTouchOutside(false);

                // Customize button colors
                Button negativeButton = datePickerDialog.getButton(DialogInterface.BUTTON_NEGATIVE);
                Button positiveButton = datePickerDialog.getButton(DialogInterface.BUTTON_POSITIVE);

                positiveButton.setTextColor(ContextCompat.getColor(TrainDetail.this, R.color.primary));
                negativeButton.setVisibility(View.GONE);
            }
        });

    }
    private void getSheduleId(String selectedShedule,SheduleCallback sheduleCallback){

        String sheduleToFilter = selectedShedule;
        String filterValue = "shedule=\"" + sheduleToFilter + "\"";

        RetrofitService retrofitService = new RetrofitService();
        SheduleApi getShedule = retrofitService.getRetrofit().create(SheduleApi.class);
        Call<SheduleRes> call = getShedule.getAllShedules(filterValue,"id",token);


        call.enqueue(new Callback<SheduleRes>() {
            @Override
            public void onResponse(Call<SheduleRes> call, Response<SheduleRes> response) {
                if (response.isSuccessful()) {
                    if (response.body() != null) {
                        SheduleRes sheduleRes = response.body();
                        SheduleItem sheduleItem = sheduleRes.getItems().get(0);
                        sheduleCallback.onLoadGetSheduleData(sheduleItem);
                    }
                } else {
                    Toast.makeText(getApplicationContext(), "Invalid Credentials", Toast.LENGTH_SHORT).show();
//                    progressLoader.dismissProgressLoader();
                }
            }
            @Override
            public void onFailure(Call<SheduleRes> call, Throwable t) {
                Toast.makeText(getApplicationContext(), "Invalid Down", Toast.LENGTH_SHORT).show();
            }
        });
    }
}