package com.example.letsbook.Activity;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.splashscreen.SplashScreen;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentTransaction;

import android.os.Bundle;
import android.view.MenuItem;

import com.example.letsbook.Fragment.IndexFragment;
import com.example.letsbook.Fragment.ProfileFragment;
import com.example.letsbook.Fragment.ReservationFragment;
import com.example.letsbook.Fragment.TrainFragment;
import com.example.letsbook.Modal.UserRecord;
import com.example.letsbook.R;
import com.google.android.material.bottomnavigation.BottomNavigationView;

public class Home extends AppCompatActivity {
    private BottomNavigationView bottomNavigationView;
    // Define constants for resource IDs
    private static final int PROFILE_ID = R.id.profile;
    private static final int HOME_ID = R.id.homie;
    private static final int TRAINS_ID = R.id.trains;
    private static final int RESERVATION_ID = R.id.reservation;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_home);

        UserRecord receivedUser = (UserRecord) getIntent().getSerializableExtra("user");

        replaceFrag(new IndexFragment(),receivedUser);
        System.out.println("this is it Home: "+receivedUser.getRecord().getEmail());

        bottomNavigationView = findViewById(R.id.bottomNavigationView);
        bottomNavigationView.setOnItemSelectedListener(item -> {
            int itemId = item.getItemId();
            switch (itemId) {
                case PROFILE_ID:
                    replaceFrag(new ProfileFragment(),receivedUser);
                    break;
                case HOME_ID:
                    replaceFrag(new IndexFragment(),receivedUser);
                    break;
                case TRAINS_ID:
                    replaceFrag(new TrainFragment(),receivedUser);
                    break;
                case RESERVATION_ID:
                    replaceFrag(new ReservationFragment(),receivedUser);
                    break;
            }
            return true;
        });

    }
    private void replaceFrag(Fragment fragment,UserRecord record) {
        FragmentManager fragmentManager = getSupportFragmentManager();
        FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();

        if (record != null) {
            Bundle bundle = new Bundle();
            bundle.putSerializable("user", record);
            fragment.setArguments(bundle);
        }

        fragmentTransaction.replace(R.id.frame_layout, fragment);
        fragmentTransaction.commit();
    }

}