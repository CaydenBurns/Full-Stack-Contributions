import React, { useState, useEffect, useRef } from "react";
import service from "services/stripeService";
import logger from "sabio-debug";
import PropTypes from "prop-types";
import { loadStripe } from "@stripe/stripe-js";
import { useLocation } from "react-router-dom";
import Card from "react-bootstrap/Card";
import ListGroup from "react-bootstrap/ListGroup";
import CancelledImg from "../../assets/images/stripe/Cancellation_Image.jpg";
import FeedHorse from "../../assets/images/stripe/horse_food.jfif";
import "../Stripe/stripe.css"
import HorseGif from "../../assets/images/stripe/horse_gif.gif";

const _logger = logger.extend("UserCheckout");



const ProductDisplay = ({ checkout }) => {
  return (
    <div className=" h-50  m-5 p-5 d-flex justify-content-center align-items-center border border-primary mx-23">
      <Card className=" justify-content-evenly align-items-center">
        <Card.Img
          className="fluid horseFeed rounded-circle border border-primary"
          variant="top"
          src={FeedHorse}
        />
        <Card.Body>
          <Card.Title>
            <h1>Horse Feed</h1>
          </Card.Title>
          <Card.Text>
            <p>
              <strong className="text-primary">Description:</strong> 1 Month
              Supply of horse feed
            </p>
            <p>
              <strong className="text-primary"> Price:</strong> $100
            </p>
          </Card.Text>
          <button
            className="btn btn-primary"
            onClick={checkout}
          >
            Checkout
          </button>
        </Card.Body>
      </Card>
    </div>
  );
};

const Message = ({ message }) => {
  const dataFetchedRef = useRef(false);
  const [subSuccessData, setSubSuccessData] = useState({
    userName: "",
    userEmail: "",
    userCountry: "",
    userRef: "",
    userCustNum: "",
    userSubExpiration: "",
  });

  useEffect(() => {
    if (window.location.search) {
      if (dataFetchedRef.current) return;
      dataFetchedRef.current = true;
      let sessionId = window.location.search.split("=");
      sessionId = sessionId[1];
      getMySession(sessionId);
    }
  }, []);

  const getMySession = (sessionId) => {
    service
      .getCheckoutSession(sessionId)
      .then(onGetSessionSuccess)
      .catch(onGetSessionError);
  };

  const onGetSessionSuccess = (response) => {
    _logger(response);
    setSubSuccessData((pState) => {
      let data = { ...pState };
      data.userName = response.item.customerDetails.name;
      data.userEmail = response.item.customerDetails.email;
      data.userCountry = response.item.customerDetails.address.country;
      data.userRef = response.item.paymentIntentId;
      data.userCustNum = response.item.customerId;
      data.userSubExpiration = response.item.expiresAt.split("T")[0];
      return data;
    });
  };

  const onGetSessionError = (error) => {
    _logger(error);
  };

  return message ===
    "A great big thank you for shopping with MiVet. We love our customers dearly, and hearing your feedback is so helpful to us. If you have a few minutes to answer a quick survey, we would be very grateful." ? (
    <div className="d-flex align-items-center justify-content-center">
      <Card className="w-50 m-5 p-5 d-flex align-items-center border border-primary">
        <div className="d-inline-flex">
          <img
            src={HorseGif}
            alt="horse gif"
            className=" d-flex rounded-circle justify-content-start "
          ></img>
        </div>
        <strong className="text-primary align-items-center justify-content-center m-3">
          {" "}
          Purchase Details
        </strong>
        <Card.Body>
          <div>
            <Card.Title className="d-flex justify-content-between text-primary">
              <div className="justify-content-start m-3">
                {subSuccessData.userName}
              </div>{" "}
              <section className="justify-content-end">
                {subSuccessData.userEmail}
              </section>
            </Card.Title>
          </div>

          <Card.Text>
            <strong>{message}</strong>
          </Card.Text>
        </Card.Body>
        <ListGroup className="list-group-flush">
          <ListGroup.Item>
            {" "}
            <strong className="">Country of origin:</strong>{" "}
            <span className="text-primary mx-3">
              {subSuccessData.userCountry}
            </span>
          </ListGroup.Item>
          <ListGroup.Item>
            <strong>Purchase reference number:</strong>{" "}
            <span className="text-primary">{subSuccessData.userRef}</span>{" "}
          </ListGroup.Item>
          <ListGroup.Item>
            <strong>Expires on : </strong>
            <span className="text-primary">
              {subSuccessData.userSubExpiration}
            </span>{" "}
          </ListGroup.Item>
        </ListGroup>
        <Card.Body>
          <Card.Link href="/">Home</Card.Link>
          <Card.Link href="/faq">FAQ</Card.Link>
        </Card.Body>
      </Card>
    </div>
  ) : (
    <Card className="   m-5 p-5 d-flex align-items-center  justify-content-center ">
      <strong className="text-primary   m-3"> Cancellation Details</strong>
      <Card.Body className="  mb-4  ">
        <img
          className=" mx-13 mb-4  border border-primary  rounded-circle"
          alt="cancel"
          src={CancelledImg}
        />

        <Card.Text>
          <strong className="text-primary">{message}</strong>
        </Card.Text>
      </Card.Body>
      <Card.Body>
        <Card.Link href="/">Home</Card.Link>
        <Card.Link href="/faq">FAQ</Card.Link>
      </Card.Body>
    </Card>
  );
};

const PUBLIC_KEY = process.env.REACT_APP_STRIPE_PUBLIC_API_KEY;
const stripePromise = loadStripe(PUBLIC_KEY);

export default function UserCheckout() {
  const [message, setMessage] = useState("");

  const [product] = useState([
    {
      id: "prod_MTD1f1VVsRIpRh",
      object: "product",
      active: true,
      DefaultPriceid: "price_1LkGb2IdaJH1yFA5Jt6seLSe",
      description: null,
      images: [],
      livemode: false,
      metadata: {},
      name: "Harry Potter",
      packageDimensions: null,
      shippable: null,
      statementDescriptor: null,
      taxCode: null,
      unitLabel: null,
      url: null,
    },
  ]);

  const onCheckoutClicked = () => {
    _logger("onCheckoutClicked");
    service
      .checkoutSession(product)
      .then(onCheckoutSuccess)
      .catch(onCheckoutError);
  };

  const onCheckoutSuccess = async (response) => {
    _logger("onCheckoutSuccess", response);
    const stripe = await stripePromise;

    stripe.redirectToCheckout({ sessionId: response.item });
  };

  const onCheckoutError = (error) => {
    _logger("onCheckoutError", error);
  };

  const location = useLocation();

  useEffect(() => {
    _logger("LOC:", location);
    let message = location.search
      ? "A great big thank you for shopping with MiVet. We love our customers dearly, and hearing your feedback is so helpful to us. If you have a few minutes to answer a quick survey, we would be very grateful."
      : location.pathname.includes("cancelled")
      ? "Order cancelled -- continue to shop around and checkout when you're ready."
      : undefined;

    setMessage(message);
  }, [location.pathname]);

  return message ? (
    <Message message={message} />
  ) : (
    <ProductDisplay checkout={onCheckoutClicked} />
  );
}

ProductDisplay.propTypes = {
  checkout: PropTypes.func.isRequired,
};

Message.propTypes = {
  message: PropTypes.string.isRequired,
};
