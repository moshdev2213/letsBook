package com.example.letsbook.Fragment;

import android.app.Dialog;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.os.Bundle;

import androidx.cardview.widget.CardView;
import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.fragment.app.Fragment;

import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.widget.Button;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import com.example.letsbook.Activity.EditProfile;
import com.example.letsbook.Activity.SignIn;
import com.example.letsbook.ApiRoutes.AuthApi;
import com.example.letsbook.ApiRoutes.EditUserApi;
import com.example.letsbook.ApiRoutes.ReservationApi;
import com.example.letsbook.CallbBacks.UserCallback;
import com.example.letsbook.DialogAlerts.ProgressLoader;
import com.example.letsbook.Modal.User;
import com.example.letsbook.Modal.UserRecord;
import com.example.letsbook.ModalDao.ReservationItem;
import com.example.letsbook.ModalDao.ReservationReq;
import com.example.letsbook.ModalDao.UserItem;
import com.example.letsbook.R;
import com.example.letsbook.RetroftService.RetrofitService;
import com.google.android.material.floatingactionbutton.FloatingActionButton;

import java.util.concurrent.CompletableFuture;
import java.util.concurrent.atomic.AtomicReference;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ProfileFragment extends Fragment {
    private ProgressLoader progressLoader;
    private UserRecord  out;
    private ConstraintLayout fragmentProfileClayout;
    private FloatingActionButton fabEditMe;
    private CardView cvProceedTodlt;
    private CardView cvLogoutUser;
    private TextView tvUserEmail;
    private TextView tvUserTel;
    private User dupUser;
    private CompletableFuture<User> userFuture ;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_profile, container, false);

        out = (UserRecord) getArguments().getSerializable("user");
        System.out.println("this is it ProFrag: "+out.getRecord().getEmail());

        fragmentProfileClayout = view.findViewById(R.id.fragmentProfileClayout);
        fragmentProfileClayout.setVisibility(View.GONE);

        fetchUser(new UserCallback() {
            @Override
            public void onLoadGetUserData(User user) {
                dupUser = user;
            }
        });

        cvLogoutUser = view.findViewById(R.id.cvLogoutUser);
        cvProceedTodlt = view.findViewById(R.id.cvProceedTodlt);
        tvUserTel = view.findViewById(R.id.tvUserTel);
        tvUserEmail = view.findViewById(R.id.tvUserEmail);
        fabEditMe = view.findViewById(R.id.fabEditMe);

        cvLogoutUser.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                startActivity(new Intent(requireActivity(), SignIn.class));
                requireActivity().finish();
            }
        });
        fabEditMe.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                fetchUser(user -> {

                    Intent intent = new Intent(requireActivity(), EditProfile.class);
                    intent.putExtra("user", user);
                    intent.putExtra("token", out.getToken()); // Add the String data// Assuming "user" is Parcelable or Serializable
                    startActivity(intent);
                });
            }
        });
        cvProceedTodlt.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Dialog dialog = new Dialog(requireActivity());
                dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
                dialog.setContentView(R.layout.confirmation_dialog);

                Button btnCancelDialog = dialog.findViewById(R.id.btnCancelDialog); // Replace "btnOk" with the actual ID of your "Ok" button
                Button btnDltConfirmDialog = dialog.findViewById(R.id.btnDltConfirmDialog);
                btnDltConfirmDialog.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        RetrofitService retrofitService = new RetrofitService();
                        EditUserApi getList = retrofitService.getRetrofit().create(EditUserApi.class);
                        Call<UserItem> call = getList
                                .deactivateUserAccount(
                                        out.getToken(),
                                        dupUser.getItems().get(0).getId(),
                                        new UserItem(
                                                dupUser.getItems().get(0).getEmail(),
                                                dupUser.getItems().get(0).getId(),
                                                dupUser.getItems().get(0).getName(),
                                                dupUser.getItems().get(0).getPhone(),
                                                dupUser.getItems().get(0).getNic(),
                                                false
                                        )
                                        );
                        call.enqueue(new Callback<UserItem>() {
                            @Override
                            public void onResponse(Call<UserItem> call, Response<UserItem> response) {
                                if (response.isSuccessful()) {
                                    if (response.body() != null) {
                                        UserItem userItem = response.body();
                                        dialog.dismiss();
                                        Intent intent = new Intent(requireActivity(), SignIn.class); // Add the String data// Assuming "user" is Parcelable or Serializable
                                        startActivity(intent);
                                        requireActivity().finish();
                                    }
                                } else {
                                    Toast.makeText(requireActivity(), "Invalid Credentials", Toast.LENGTH_SHORT).show();
//                    progressLoader.dismissProgressLoader();
                                }
                            }
                            @Override
                            public void onFailure(Call<UserItem> call, Throwable t) {
                                Toast.makeText(requireActivity(), "Invalid Down", Toast.LENGTH_SHORT).show();
                            }
                        });
                    }
                });
                btnCancelDialog.setOnClickListener(new View.OnClickListener() {
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
        });

        return view;
    }
    private void fetchUser(UserCallback userCallback) {
            progressLoader = new ProgressLoader(requireContext(), "Fetching details", "Please Wait");
            progressLoader.startProgressLoader();

            RetrofitService retrofitService = new RetrofitService();
            AuthApi authService = retrofitService.getRetrofit().create(AuthApi.class);

            String emailToFilter = out.getRecord().getEmail();
            String filterValue = "email=\"" + emailToFilter + "\"";
            Call<User> call = authService.getUserDetail(filterValue, out.getToken());
            call.enqueue(new Callback<User>() {
                @Override
                public void onResponse(Call<User> call, Response<User> response) {
                    System.out.println("this is it ProFrag Readind ResBosy: "+response.body());
                    if (response.isSuccessful()) {
                        User user = response.body();
                        if (user != null) {
                            userCallback.onLoadGetUserData(user);
                            UserItem firstUserItem = user.getItems().get(0);
                            progressLoader.dismissProgressLoader();
                            tvUserEmail.setText(firstUserItem.getEmail());
                            tvUserTel.setText(" Tel : "+firstUserItem.getPhone());
                            fragmentProfileClayout.setVisibility(View.VISIBLE);
                        }
                    } else {
                        Toast.makeText(requireActivity(), "Invalid Credentials", Toast.LENGTH_SHORT).show();
                        progressLoader.dismissProgressLoader();
                    }
                }

                @Override
                public void onFailure(Call<User> call, Throwable t) {
                    System.out.println("Error: " + t.getMessage()); // Print the error message
                    Toast.makeText(requireActivity(), "Server Error", Toast.LENGTH_SHORT).show();
                    progressLoader.dismissProgressLoader();
                }
            });
        }
}