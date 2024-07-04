package com.example.letsbook.Adapter;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.cardview.widget.CardView;
import androidx.recyclerview.widget.RecyclerView;

import com.example.letsbook.ModalDao.ReservationItem;
import com.example.letsbook.ModalDao.TrainItem;
import com.example.letsbook.R;

import java.util.ArrayList;
import java.util.List;

public class ReservationAdapter extends RecyclerView.Adapter<ReservationAdapterViewHolder>{
    private final List<ReservationItem> reservationList = new ArrayList<>();
    private final ReservationAdapter.OnReservationItemClickListener reservationCardClicked;
    private final ReservationAdapter.OnReservationItemClickListener reservationEditCardClicked;
    private final ReservationAdapter.OnReservationItemClickListener reservationCancelCardClicked;

    public ReservationAdapter(OnReservationItemClickListener reservationCardClick, OnReservationItemClickListener reservationEditCardClicked, OnReservationItemClickListener reservationCancelCardClicked) {
        this.reservationCardClicked = reservationCardClick;
        this.reservationEditCardClicked = reservationEditCardClicked;
        this.reservationCancelCardClicked = reservationCancelCardClicked;
    }

    @NonNull
    @Override
    public ReservationAdapterViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View listItem = LayoutInflater.from(parent.getContext()).inflate(R.layout.reservation_item, parent, false);
        return new ReservationAdapterViewHolder(listItem);
    }

    @Override
    public void onBindViewHolder(@NonNull ReservationAdapterViewHolder holder, int position) {
        holder.bind(reservationList.get(position), reservationCardClicked,reservationEditCardClicked,reservationCancelCardClicked);
    }

    @Override
    public int getItemCount() {
        return reservationList.size();
    }
    public void setList(List<ReservationItem> reservationItems) {
        reservationList.clear();
        reservationList.addAll(reservationItems);
        notifyDataSetChanged();
    }

    public interface OnReservationItemClickListener {
        void onItemClick(ReservationItem reservationItem);
    }

}
class ReservationAdapterViewHolder extends RecyclerView.ViewHolder {
    public ReservationAdapterViewHolder(View view) {
        super(view);
    }

    public void bind(
            ReservationItem reservationItem,
            ReservationAdapter.OnReservationItemClickListener reservationCardClicked,
            ReservationAdapter.OnReservationItemClickListener reservationEditCardClicked,
            ReservationAdapter.OnReservationItemClickListener reservationCancelCardClicked
    ) {
        CardView cvEditReserveItem = itemView.findViewById(R.id.cvEditReserveItem);
        CardView cvCancelReserveItem = itemView.findViewById(R.id.cvCancelReserveItem);
        CardView cvMainPayCardReservation = itemView.findViewById(R.id.cvMainPayCardReservation);

        TextView tvMainNameDis = itemView.findViewById(R.id.tvMainNameDis);
        TextView tvPayDate = itemView.findViewById(R.id.tvPayDate);
        TextView tvPayTime = itemView.findViewById(R.id.tvPayTime);

        tvMainNameDis.setText(reservationItem.getEmail());
        tvPayDate.setText(reservationItem.getReserved());
        tvPayTime.setText(reservationItem.getSeats()+" Seats");

        cvEditReserveItem.setOnClickListener(v -> reservationEditCardClicked.onItemClick(reservationItem));
        cvCancelReserveItem.setOnClickListener(v -> reservationCancelCardClicked.onItemClick(reservationItem));

        cvMainPayCardReservation.setOnClickListener(v -> reservationCardClicked.onItemClick(reservationItem));
    }
}
