import React from "react";
import Button from "react-bootstrap/Button";
import Card from "react-bootstrap/Card";
import "../Stripe/stripe.css"
const viewSubscription = ({  sub, subscribe }) => {
  
  const onSubscribeClicked = (e) => {
    subscribe(sub, e);
  };

  return (
    <Card className="m-5 p-5 cardView border border-primary ">
      <Card.Img className="viewSubscriptionImg rounded-circle"  src={sub.imageUrl} />
      <Card.Body>
        <Card.Title>{sub.name}</Card.Title>
        <Card.Text></Card.Text>
        <Button onClick={onSubscribeClicked} variant="primary">
          Subscribe
        </Button>
      </Card.Body>
    </Card>
  );
};

export default viewSubscription;