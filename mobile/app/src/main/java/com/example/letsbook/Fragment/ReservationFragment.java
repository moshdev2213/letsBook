package com.example.letsbook.Fragment;

import android.app.Dialog;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.os.Bundle;

import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.widget.Button;
import android.widget.Spinner;
import android.widget.Toast;

import com.example.letsbook.Adapter.ReservationAdapter;
import com.example.letsbook.Adapter.TrainAdapter;
import com.example.letsbook.ApiRoutes.ReservationApi;
import com.example.letsbook.ApiRoutes.TrainApi;
import com.example.letsbook.CallbBacks.ResrvationCallback;
import com.example.letsbook.Modal.UserRecord;
import com.example.letsbook.ModalDao.ReservationItem;
import com.example.letsbook.ModalDao.ReservationReq;
import com.example.letsbook.ModalDao.ReservationRes;
import com.example.letsbook.ModalDao.TrainItem;
import com.example.letsbook.ModalDao.TrainRes;
import com.example.letsbook.R;
import com.example.letsbook.RetroftService.RetrofitService;

import java.util.Calendar;
import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ReservationFragment extends Fragment {
    private RecyclerView rvReservationFrag;
    private ReservationAdapter reservationAdapter;
    private UserRecord out;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_reservation, container, false);

        out = (UserRecord) getArguments().getSerializable("user");
        System.out.println("this is it ReserveFrag: "+out.getRecord().getEmail());
        initRecycler(view);
        return view;
    }

    private void initRecycler(View view) {
        rvReservationFrag = view.findViewById(R.id.rvReservationFrag);
        rvReservationFrag.setLayoutManager(new LinearLayoutManager(requireActivity()));

        reservationAdapter = new ReservationAdapter(
                new ReservationAdapter.OnReservationItemClickListener() {
                    @Override
                    public void onItemClick(ReservationItem reservationItem) {
                        mainCardClicked(reservationItem);
                    }
                },
                new ReservationAdapter.OnReservationItemClickListener() {
                    @Override
                    public void onItemClick(ReservationItem reservationItem) {
                        editCardClicked(reservationItem);
                    }
                },
                new ReservationAdapter.OnReservationItemClickListener() {
                    @Override
                    public void onItemClick(ReservationItem reservationItem) {
                        cancelCardClicked(reservationItem);
                    }
                }
        );
        rvReservationFrag.setAdapter(reservationAdapter);
        fetchDetails();
    }

    private void mainCardClicked(ReservationItem reservationItem){
        Toast.makeText(getActivity(),"Main",Toast.LENGTH_SHORT).show();
    }
    private void editCardClicked(ReservationItem reservationItem){
        // Get the current date
        Calendar calendar = Calendar.getInstance();
        int currentYear = calendar.get(Calendar.YEAR);
        int currentMonth = calendar.get(Calendar.MONTH) + 1; // Note: Months are 0-based in Calendar, so add 1
        int currentDay = calendar.get(Calendar.DAY_OF_MONTH);

        // Parse the date string from the reservationItem (e.g., "2023.10.24")
        String[] dateParts = reservationItem.getReserved().split("\\.");
        if (dateParts.length == 3) {
            int year = Integer.parseInt(dateParts[0]);
            int month = Integer.parseInt(dateParts[1]);
            int day = Integer.parseInt(dateParts[2]);

            // Calculate the date difference in days
            int dateDifference = calculateDateDifference(currentYear, currentMonth, currentDay, year, month, day);

            if (dateDifference <= 30) {
                Dialog dialog = new Dialog(requireActivity());
                dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
                dialog.setContentView(R.layout.edit_reservation);

                Button btnDlt = dialog.findViewById(R.id.btnDltResEdit); // Replace "btnOk" with the actual ID of your "Ok" button
                Button btnCancel = dialog.findViewById(R.id.btnCancelResEdit); // Replace "btnNo" with the actual ID of your "No" button
                Spinner spinnerSeatEdit = dialog.findViewById(R.id.spinnerSeatEdit);
                btnDlt.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        RetrofitService retrofitService = new RetrofitService();
                        ReservationApi getList = retrofitService.getRetrofit().create(ReservationApi.class);
                        Call<ReservationItem> call = getList
                                .editReservation(
                                        out.getToken(),
                                        reservationItem.getId(),
                                        new ReservationReq(
                                                reservationItem.getTrainId(),
                                                reservationItem.getSheduleId(),
                                                reservationItem.getEmail(),
                                                reservationItem.getReserved(),
                                                Integer.parseInt(spinnerSeatEdit.getSelectedItem().toString()),
                                                reservationItem.getCanceled()
                                        ));
                        call.enqueue(new Callback<ReservationItem>() {
                            @Override
                            public void onResponse(Call<ReservationItem> call, Response<ReservationItem> response) {
                                if (response.isSuccessful()) {
                                    if (response.body() != null) {
                                        ReservationItem reserveItem = response.body();
                                        dialog.dismiss();
                                        Toast.makeText(requireActivity(), "Reservation Edited", Toast.LENGTH_SHORT).show();
                                    }
                                } else {
                                    Toast.makeText(requireActivity(), "Invalid Credentials", Toast.LENGTH_SHORT).show();
//                    progressLoader.dismissProgressLoader();
                                }
                            }
                            @Override
                            public void onFailure(Call<ReservationItem> call, Throwable t) {
                                Toast.makeText(requireActivity(), "Invalid Down", Toast.LENGTH_SHORT).show();
                            }
                        });
                    }
                });
                btnCancel.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        // Handle the "No" button click
                        // You can perform actions or dismiss the dialog here
                        dialog.dismiss(); // Close the dialog, for example
                    }
                });

                dialog.show();
                dialog.setCanceledOnTouchOutside(false); // Prevent canceling when clicked outside

                Window window = dialog.getWindow();
                if (window != null) {
                    window.setLayout(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.WRAP_CONTENT);
                    window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
                    window.setGravity(Gravity.BOTTOM);
                }
                //the notifydatasetChanged
            } else {
                // The date difference is more than 30 days
                Toast.makeText(requireContext(), "Date Exceeded 30 days", Toast.LENGTH_SHORT).show();
            }
        } else {
            // Invalid date format in reservationItem.getDate()
            Toast.makeText(requireContext(), "Invalid Date Format", Toast.LENGTH_SHORT).show();
        }
    }
    private void cancelCardClicked(ReservationItem reservationItem){
        System.out.println(reservationItem.getId());

        Dialog dialog = new Dialog(requireActivity());
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        dialog.setContentView(R.layout.bottom_sheet_layout);

        Button btnDlt = dialog.findViewById(R.id.btnDltConfirm); // Replace "btnOk" with the actual ID of your "Ok" button
        Button btnCancel = dialog.findViewById(R.id.btnCancelConfirm); // Replace "btnNo" with the actual ID of your "No" button

        btnDlt.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                RetrofitService retrofitService = new RetrofitService();
                ReservationApi getList = retrofitService.getRetrofit().create(ReservationApi.class);
                Call<ReservationItem> call = getList
                        .cancelReservation(
                                out.getToken(),
                                reservationItem.getId(),
                                new ReservationReq(
                                        reservationItem.getTrainId(),
                                        reservationItem.getSheduleId(),
                                        reservationItem.getEmail(),
                                        reservationItem.getReserved(),
                                        reservationItem.getSeats(),
                                        1
                                ));
                call.enqueue(new Callback<ReservationItem>() {
                    @Override
                    public void onResponse(Call<ReservationItem> call, Response<ReservationItem> response) {
                        if (response.isSuccessful()) {
                            if (response.body() != null) {
                                ReservationItem reserveItem = response.body();
                                dialog.dismiss();
                                Toast.makeText(requireActivity(), "Reservation Canceled", Toast.LENGTH_SHORT).show();
                            }
                        } else {
                            Toast.makeText(requireActivity(), "Invalid Credentials", Toast.LENGTH_SHORT).show();
//                    progressLoader.dismissProgressLoader();
                        }
                    }
                    @Override
                    public void onFailure(Call<ReservationItem> call, Throwable t) {
                        Toast.makeText(requireActivity(), "Invalid Down", Toast.LENGTH_SHORT).show();
                    }
                });
            }
        });

        btnCancel.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Handle the "No" button click
                // You can perform actions or dismiss the dialog here
                dialog.dismiss(); // Close the dialog, for example
            }
        });

        dialog.show();
        dialog.setCanceledOnTouchOutside(false); // Prevent canceling when clicked outside

        Window window = dialog.getWindow();
        if (window != null) {
            window.setLayout(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.WRAP_CONTENT);
            window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
            window.setGravity(Gravity.BOTTOM);
        }
    }

    private void fetchDetails() {
        RetrofitService retrofitService = new RetrofitService();
        ReservationApi getList = retrofitService.getRetrofit().create(ReservationApi.class);
        Call<ReservationRes> call = getList.getAllReservations(out.getToken());
        System.out.println("Im train Frag "+out.getToken());
        call.enqueue(new Callback<ReservationRes>() {
            @Override
            public void onResponse(Call<ReservationRes> call, Response<ReservationRes> response) {
                if (response.isSuccessful()) {
                    if (response.body() != null) {
                        ReservationRes reserveRes = response.body();
                        List<ReservationItem> trainItem = reserveRes.getItems();
                        reservationAdapter.setList(trainItem);
                    }
                } else {
                    Toast.makeText(requireActivity(), "Invalid Credentials", Toast.LENGTH_SHORT).show();
//                    progressLoader.dismissProgressLoader();
                }
            }
            @Override
            public void onFailure(Call<ReservationRes> call, Throwable t) {
                Toast.makeText(requireActivity(), "Invalid Down", Toast.LENGTH_SHORT).show();
            }
        });
    }

    // Calculate the date difference in days
    private int calculateDateDifference(int currentYear, int currentMonth, int currentDay, int year, int month, int day) {
        Calendar currentDate = Calendar.getInstance();
        currentDate.set(currentYear, currentMonth - 1, currentDay); // Note: Months are 0-based
        Calendar targetDate = Calendar.getInstance();
        targetDate.set(year, month - 1, day); // Note: Months are 0-based

        long currentTimeInMillis = currentDate.getTimeInMillis();
        long targetTimeInMillis = targetDate.getTimeInMillis();

        // Calculate the date difference in days
        long dateDifferenceInMillis = targetTimeInMillis - currentTimeInMillis;
        return (int) (dateDifferenceInMillis / (1000 * 60 * 60 * 24));
    }
}