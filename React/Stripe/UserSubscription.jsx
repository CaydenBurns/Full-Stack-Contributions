import React, { useState, useEffect, useRef } from "react";
import service from "services/stripeService";
import debug from "sabio-debug";
import PropTypes from "prop-types";
import { loadStripe } from "@stripe/stripe-js";
import { useLocation } from "react-router-dom";
import MiVet from "../../assets/images/logos/MiVet-Logo_BUG.png";
import ViewSubscription from "./ViewSubscription";
import Card from "react-bootstrap/Card";
import ListGroup from "react-bootstrap/ListGroup";
import toastr from "toastr";
import CancelledImg from "../../assets/images/stripe/Cancellation_Image.jpg";
import HorseGif from "../../assets/images/stripe/horse_gif.gif";

const _logger = debug.extend("UserSubscription");

const ProductDisplay = ({ subscribe }) => {
  const [subscription, setSubscription] = useState([]);
  const dataFetchedRef = useRef(false);
  const serviceCall = () => {
    service
      .getSubscriptions()
      .then(onGetSubscriptionsSucces)
      .catch(onGetSubscriptionsError);
  };

  useEffect(() => {
    if (dataFetchedRef.current) return;
    dataFetchedRef.current = true;
    serviceCall();
  }, []);

  const onGetSubscriptionsSucces = (response) => {
    _logger("onGetSubscriptionsSucces", response);
    let newSubs = response.items;
    setSubscription(newSubs);
  };

  const onGetSubscriptionsError = (error) => {
    _logger(error);
    toastr.error(
      "Unable to retrieve MiVet Subscriptions. Please contact your Administrator"
    );
  };

  const onConnectClicked = () => {
    service
      .connectExpress()
      .then(onConnectExpressSuccess)
      .catch(onConnectExpressError);
  };

  const onConnectExpressSuccess = (response) => {
    _logger("onConnectExpressSuccess", response);
    let url = JSON.stringify(response.item.id);

    _logger(url);
    service
      .expressLink(url)
      .then(onExpressLinkSuccess)
      .catch(onExpressLinkError);
  };

  const onConnectExpressError = (error) => {
    _logger(error);
    toastr.error(error);
  };

  const onExpressLinkSuccess = (response) => {
    _logger("onExpressLinkSuccess", response);
    window.location.href = response.item.url;
  };

  const onExpressLinkError = (error) => {
    _logger(error);
    toastr.error(error);
  };

  const mapSubscriptions = (sub) => {
    return (
      <ViewSubscription
        image={MiVet}
        subscribe={subscribe}
        subscription={subscription}
        sub={sub}
        key={sub.id}
      />
    );
  };
  return (
    <div className="container">
      <div className="m-5 p-5 d-flex justify-content-evenly align-items-center">
        <button onClick={onConnectClicked} className="btn btn-primary">
          Become A Provider
        </button>
       
      </div>
      <strong className="m-5 p-5 d-flex justify-content-center align-items-center text-primary sub">Subscriptions</strong>
      <div className=" m-5 p-5 d-flex justify-content-evenly align-items-center ">
        {subscription.map(mapSubscriptions)}
      </div>
    </div>
  );
};
const Message = ({ message }) => {
  const onGetSessionSuccess = (response) => {
    _logger("onGetSessionSuccess", response);
    setSubSuccessData((pState) => {
      let data = { ...pState };
      data.userName = response.item.customerDetails.name;
      data.userEmail = response.item.customerDetails.email;
      data.userCountry = response.item.customerDetails.address.country;
      data.userRef = response.item.subscriptionId;
      data.userCustNum = response.item.customerId;
      data.userSubExpiration = response.item.expiresAt.split("T")[0];
      return data;
    });
    _logger("subSuccessData", subSuccessData);
    _logger("username", response.item.customerDetails.name);
    _logger("username", response.item.customerDetails.email);
  };

  const onGetSessionError = (error) => {
    _logger("onGetSessionError", error);
  };
  const getMySession = (sessionId) => {
    service
      .getSession(sessionId)
      .then(onGetSessionSuccess)
      .catch(onGetSessionError);
  };

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
            <strong>Customer reference number:</strong>{" "}
            <span className="text-primary">{subSuccessData.userCustNum}</span>{" "}
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

_logger(PUBLIC_KEY);
const stripePromise = loadStripe(PUBLIC_KEY);
_logger(stripePromise);

export default function UserSubscription() {
  const [message, setMessage] = useState("");

  const onSubscribeClicked = (subCard) => {
    let payload = {
      priceId: subCard.priceId,
      customer: {
        customer: "Mivet Customer",
      },
      productId: subCard.productId,
    };

    service
      .createCustomer(payload)
      .then(onCreateCustomerSuccess)
      .catch(onCreateCustomerError);
  };

  const onCreateCustomerSuccess = (response) => {
    _logger("onCreateCustomerSuccess, RESPONSE", response);

    let stripeCustomer = { userId: 0, customerId: response.data.item.id };
    let price = response.priceId;
    let productId = response.productId;
    let payload = {
      priceId: price,
      customerId: stripeCustomer.customerId,
      productId: productId,
    };

    service
      .subscriptionService(payload)
      .then(onSubscribeSuccess)
      .catch(onSubscribeError);
  };

  const onCreateCustomerError = (error) => {
    _logger("onCreateCustomerErrorr", error);
    toastr.error(error);
  };

  const onSubscribeSuccess = async (response) => {
    _logger("ProductIdPRODICUT", response);

    const stripe = await stripePromise;

    stripe.redirectToCheckout({ sessionId: response.data.item });
  };

  const onSubscribeError = (error) => {
    _logger("onSubscribeErrorr", error);
  };

  const location = useLocation();

  useEffect(() => {
    let message = location.search
      ? "A great big thank you for shopping with MiVet. We love our customers dearly, and hearing your feedback is so helpful to us. If you have a few minutes to answer a quick survey, we would be very grateful."
      : location.pathname.includes("cancelled")
      ? "Order cancelled. You can continue to shop around and checkout when you're ready."
      : undefined;

    setMessage(message);
  }, [location.pathname]);

  return message ? (
    <Message message={message} />
  ) : (
    <ProductDisplay subscribe={onSubscribeClicked} />
  );
}

ProductDisplay.propTypes = {
  subscribe: PropTypes.func.isRequired,
};

Message.propTypes = {
  message: PropTypes.string.isRequired,
};
