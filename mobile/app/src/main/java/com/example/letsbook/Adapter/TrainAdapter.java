package com.example.letsbook.Adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.cardview.widget.CardView;
import androidx.recyclerview.widget.RecyclerView;

import com.example.letsbook.ModalDao.TrainItem;
import com.example.letsbook.R;

import java.util.ArrayList;
import java.util.List;

public class TrainAdapter extends RecyclerView.Adapter<TrainAdapterViewHolder> {
    private final List<TrainItem> trainerList = new ArrayList<>();
    private final OnTrainItemClickListener trainCardClicked;

    public TrainAdapter(OnTrainItemClickListener mealCardClicked) {
        this.trainCardClicked = mealCardClicked;
    }

    @NonNull
    @Override
    public TrainAdapterViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View listItem = LayoutInflater.from(parent.getContext()).inflate(R.layout.train_item, parent, false);
        return new TrainAdapterViewHolder(listItem);
    }

    @Override
    public void onBindViewHolder(@NonNull TrainAdapterViewHolder holder, int position) {
        holder.bind(trainerList.get(position), trainCardClicked);
    }

    @Override
    public int getItemCount() {

        return trainerList.size();
    }
    public void setList(List<TrainItem> trainItems) {
        trainerList.clear();
        trainerList.addAll(trainItems);
        notifyDataSetChanged();
    }
    public interface OnTrainItemClickListener {
        void onItemClick(TrainItem trainItem);
    }


}
class TrainAdapterViewHolder extends RecyclerView.ViewHolder {
    public TrainAdapterViewHolder(View view) {
        super(view);
    }

    public void bind(TrainItem trainItem, TrainAdapter.OnTrainItemClickListener trainCardClicked) {
        TextView tvTypeTrain = itemView.findViewById(R.id.tvTypeTrain);
        TextView tvTrainName = itemView.findViewById(R.id.tvTrainName);
        CardView cvTrainItemCard = itemView.findViewById(R.id.cvTrainItemCard);

        tvTrainName.setText(trainItem.getTrainName().toUpperCase());
        tvTypeTrain.setText(trainItem.getTrainType().toUpperCase());
        cvTrainItemCard.setOnClickListener(v -> trainCardClicked.onItemClick(trainItem));
    }
}
