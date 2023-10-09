package com.example.letsbook.Fragment;

import android.content.Intent;
import android.os.Bundle;

import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentTransaction;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;

import com.example.letsbook.Activity.SignUp;
import com.example.letsbook.Modal.UserRecord;
import com.example.letsbook.R;
import com.google.android.material.bottomnavigation.BottomNavigationView;

public class IndexFragment extends Fragment {
    private Button btnSeeAct;
    private Button btnBrowseReserves;
    private Button btnShowTrains;
    private BottomNavigationView bottomNavigationView;
    private UserRecord out;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_index, container, false);

        btnSeeAct = view.findViewById(R.id.btnSeeAct);
        btnBrowseReserves = view.findViewById(R.id.btnBrowseReserves);
        btnShowTrains = view.findViewById(R.id.btnShowTrains);
        bottomNavigationView = getActivity().findViewById(R.id.bottomNavigationView);
        out = (UserRecord) getArguments().getSerializable("user");
        btnSeeAct.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                reDirectAccountPage();
                bottomNavigationView.setSelectedItemId(R.id.profile);
            }
        });

        btnBrowseReserves.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                reDirectAccountPage();
                bottomNavigationView.setSelectedItemId(R.id.reservation);
            }
        });

        btnShowTrains.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                reDirectAccountPage();
                bottomNavigationView.setSelectedItemId(R.id.trains);
            }
        });

        return view;
    }
    private void reDirectAccountPage(){
        replaceFrag(new ProfileFragment(),out);
    }
    private void reDirectTrainPage(){
        replaceFrag(new TrainFragment(),out);
    }
    private void reDirectReservationPage(){
        replaceFrag(new ReservationFragment(),out);
    }
    private void replaceFrag(Fragment fragment,UserRecord out) {
        FragmentTransaction transaction = getActivity().getSupportFragmentManager().beginTransaction();

        if (out != null) {
            Bundle bundle = new Bundle();
            bundle.putSerializable("user", out);
            fragment.setArguments(bundle);
        }

        transaction.replace(R.id.frame_layout, fragment); // Replace the current Fragment with FragmentB
        transaction.commit();
    }
}