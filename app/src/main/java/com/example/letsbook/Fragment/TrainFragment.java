package com.example.letsbook.Fragment;

import android.content.Intent;
import android.os.Bundle;

import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Adapter;
import android.widget.Toast;

import com.example.letsbook.Activity.TrainDetail;
import com.example.letsbook.Adapter.TrainAdapter;
import com.example.letsbook.ApiRoutes.TrainApi;
import com.example.letsbook.Modal.User;
import com.example.letsbook.Modal.UserRecord;
import com.example.letsbook.ModalDao.TrainItem;
import com.example.letsbook.ModalDao.TrainRes;
import com.example.letsbook.ModalDao.UserItem;
import com.example.letsbook.R;
import com.example.letsbook.RetroftService.RetrofitService;

import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class TrainFragment extends Fragment {
    private RecyclerView rvTrainFrag;
    private TrainAdapter trainAdapter;
    private UserRecord  out;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view =inflater.inflate(R.layout.fragment_train, container, false);

        out = (UserRecord) getArguments().getSerializable("user");
        System.out.println("this is it TrainFrag: "+out.getRecord().getEmail());
        initRecycler(view);
        return view;
    }
    private void initRecycler(View view) {
        rvTrainFrag = view.findViewById(R.id.rvTrainFrag);
        rvTrainFrag.setLayoutManager(new LinearLayoutManager(requireActivity()));

        trainAdapter = new TrainAdapter(new TrainAdapter.OnTrainItemClickListener() {
            @Override
            public void onItemClick(TrainItem trainItem) {
                trainCardClicked(trainItem);
            }
        });

        rvTrainFrag.setAdapter(trainAdapter);
        fetchDetails();
    }

    private void fetchDetails() {
        RetrofitService retrofitService = new RetrofitService();
        TrainApi getList = retrofitService.getRetrofit().create(TrainApi.class);
        Call<TrainRes> call = getList.getAllTrain(out.getToken());
        System.out.println("Im train Frag "+out.getToken());
        call.enqueue(new Callback<TrainRes>() {
            @Override
            public void onResponse(Call<TrainRes> call, Response<TrainRes> response) {
                if (response.isSuccessful()) {
                    if (response.body() != null) {
                        TrainRes trainRes = response.body();
                        List<TrainItem> trainItem = trainRes.getItems();
                        trainAdapter.setList(trainItem);
                        Toast.makeText(requireActivity(),trainItem.get(0).getTrainName(),Toast.LENGTH_SHORT).show();
                    }
                } else {
                    Toast.makeText(requireActivity(), "Invalid Credentials", Toast.LENGTH_SHORT).show();
//                    progressLoader.dismissProgressLoader();
                }
            }
            @Override
            public void onFailure(Call<TrainRes> call, Throwable t) {
                Toast.makeText(requireActivity(), "Invalid Down", Toast.LENGTH_SHORT).show();
            }
        });
    }
    private void trainCardClicked(TrainItem trainItem){
        Bundle bundle = new Bundle();
        bundle.putSerializable("train", trainItem);
        bundle.putString("token", out.getToken());

        Intent intent = new Intent(requireActivity(), TrainDetail.class);
        intent.putExtras(bundle);
        startActivity(intent);
    }

}